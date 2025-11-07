using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.External.Clients;
using NewLifeHRT.External.Enums;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Mappings;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Factory.Provider
{
    public class EmpowerIntegrationProvider : IIntegrationProvider
    {
        private readonly EmpowerApiClient _client;
        private readonly ClinicDbContext _clinicDbContext;
        private readonly ITemplateContentGenerator _templateContentGenerator;
        private readonly IPdfConverter _pdfConverter;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;

        public EmpowerIntegrationProvider(EmpowerApiClient client, ClinicDbContext clinicDbContext, ITemplateContentGenerator templateContentGenerator, IPdfConverter pdfConverter, IOptions<AzureBlobStorageSettings> azureBlobStorageSettings)        {
            _client = client;
            _clinicDbContext = clinicDbContext;
            _templateContentGenerator = templateContentGenerator;
            _pdfConverter = pdfConverter;
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
        }


        public async Task SendOrderAsync(Guid orderId, Dictionary<string, string> configData)
        {
            _client.Initialize(configData);

            var failedTrackings = await _clinicDbContext.OrderProcessingApiTrackings
                .Where(t => t.OrderId == orderId && t.Status == OrderProcessApiTrackingStatusEnum.Failed)
                .Include(t => t.Transactions)
                .ToListAsync();

            if (failedTrackings.Any())
            {
                _clinicDbContext.OrderProcessingApiTransactions.RemoveRange(
                    failedTrackings.SelectMany(t => t.Transactions));

                _clinicDbContext.OrderProcessingApiTrackings.RemoveRange(failedTrackings);
                await _clinicDbContext.SaveChangesAsync();
            }

            var order = await _clinicDbContext.Orders
                .Include(o => o.Patient)
                .Include(o => o.Physician).ThenInclude(p => p.Address).ThenInclude(a => a.Country)
                .Include(o => o.Physician).ThenInclude(p => p.Address).ThenInclude(a => a.State)
                .Include(o => o.OrderDetails).ThenInclude(od => od.ProductPharmacyPriceListItem)
                .Include(o => o.ShippingAddress).ThenInclude(sa => sa.Address).ThenInclude(a => a.Country)
                .Include(o => o.ShippingAddress).ThenInclude(sa => sa.Address).ThenInclude(a => a.State)
                .Include(o => o.PharmacyShippingMethod).ThenInclude(sa => sa.ShippingMethod)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
                throw new InvalidOperationException($"Order with ID {orderId} was not found.");

            var tracking = new OrderProcessingApiTracking
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                IntegrationTypeId = 3,
                Status = OrderProcessApiTrackingStatusEnum.InProgress,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _clinicDbContext.OrderProcessingApiTrackings.AddAsync(tracking);
            await _clinicDbContext.SaveChangesAsync();

            try
            {
                var getTokenRequest = new EmpGetTokenModel.Request
                {
                    ApiKey = configData["ApiKey"],
                    ApiSecret = configData["ApiSecret"]
                };

                var tokenResponse = await _client.GetTokenAsync(getTokenRequest);

                var tokenTransaction = new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = tracking.Id,
                    Endpoint = "/GetToken/Post",
                    Payload = JsonSerializer.Serialize(getTokenRequest),
                    Status = tokenResponse?.Type == ResponseTypeEnum.Success
                        ? OrderProcessApiTrackingStatusEnum.Success
                        : OrderProcessApiTrackingStatusEnum.Failed,
                    ResponseMessage = tokenResponse?.Message ?? "No response message",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _clinicDbContext.OrderProcessingApiTransactions.AddAsync(tokenTransaction);

                if (tokenResponse?.Type == ResponseTypeEnum.Success)
                {
                    bool requiresScheduleCode = order.OrderDetails
                        .Any(od => od.ProductPharmacyPriceListItem?.LifeFileScheduledCodeId != null);

                    string? base64String = null;
                    if (requiresScheduleCode)
                    {
                        var model = order.ToControlledDrugOrderModel(configData, _azureBlobStorageSettings);
                        if (!string.IsNullOrEmpty(model.Prescriber.SignatureUrl))
                        {
                            using (var httpClient = new HttpClient())
                            {
                                var imgBytes = await httpClient.GetByteArrayAsync(model.Prescriber.SignatureUrl);
                                var base64Image = Convert.ToBase64String(imgBytes);
                                model.Prescriber.SignatureUrl = $"data:image/png;base64,{base64Image}";
                            }
                        }
                        var html = _templateContentGenerator.GetTemplateContent(model);
                        base64String = _pdfConverter.ConvertToPdf(html);
                    }

                    var empEasyRxRequest = order.ToEmpPostEasyRxRequestModel(configData, requiresScheduleCode, base64String);
                    var empEasyRxResponse = await _client.PostEasyRxAsync(empEasyRxRequest);

                    var easyRxTransaction = new OrderProcessingApiTransaction
                    {
                        Id = Guid.NewGuid(),
                        OrderProcessingApiTrackingId = tracking.Id,
                        Endpoint = "/NewRx/EasyRx",
                        Payload = JsonSerializer.Serialize(empEasyRxRequest),
                        Status = empEasyRxResponse?.Type == ResponseTypeEnum.Success
                            ? OrderProcessApiTrackingStatusEnum.Success
                            : OrderProcessApiTrackingStatusEnum.Failed,
                        ResponseMessage = empEasyRxResponse?.Message ?? "No response message",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await _clinicDbContext.OrderProcessingApiTransactions.AddAsync(easyRxTransaction);

                    if (empEasyRxResponse?.Type == ResponseTypeEnum.Success &&
                        !string.IsNullOrEmpty(empEasyRxResponse.Data.EipOrderId))
                    {
                        order.PharmacyOrderNumber = empEasyRxResponse.Data.EipOrderId;
                        order.Status = Domain.Enums.OrderStatus.LifeFileSuccess;
                        tracking.Status = OrderProcessApiTrackingStatusEnum.Success;
                    }
                    else
                    {
                        order.Status = Domain.Enums.OrderStatus.LifeFileError;
                        order.IsReadyForLifeFile = false;
                        tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;
                    }
                }
                else
                {
                    order.Status = Domain.Enums.OrderStatus.LifeFileError;
                    order.IsReadyForLifeFile = false;
                    tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;
                }

                await _clinicDbContext.SaveChangesAsync();
            }
            catch (HttpRequestException ex)
            {
                tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;
                await _clinicDbContext.SaveChangesAsync();
                Console.WriteLine($"Failed to send order {orderId}. Error: {ex.Message}");
            }
        }
    }
}
