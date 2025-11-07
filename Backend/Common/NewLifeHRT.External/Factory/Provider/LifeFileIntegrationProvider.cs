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
using System.Threading;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Factory.Provider
{
    public class LifeFileIntegrationProvider : IIntegrationProvider
    {
        private readonly LifeFileApiClient _client;
        private readonly ClinicDbContext _clinicDbContext;
        private readonly ITemplateContentGenerator _templateContentGenerator;
        private readonly IPdfConverter _pdfConverter;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;

        public LifeFileIntegrationProvider(LifeFileApiClient client, ClinicDbContext clinicDbContext, ITemplateContentGenerator templateContentGenerator, IPdfConverter pdfConverter, IOptions<AzureBlobStorageSettings> azureBlobStorageSettings)
        {
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
                .Include(o => o.Patient).ThenInclude(p => p.Address).ThenInclude(a => a.State)
                .Include(o => o.Physician).ThenInclude(p => p.Address).ThenInclude(a => a.State)
                .Include(o => o.OrderDetails).ThenInclude(od => od.ProductPharmacyPriceListItem)
                    .ThenInclude(ppi => ppi.LifeFileDrugForm)
                .Include(o => o.OrderDetails).ThenInclude(od => od.ProductPharmacyPriceListItem)
                    .ThenInclude(ppi => ppi.LifeFileQuantityUnit)
                .Include(o => o.OrderDetails).ThenInclude(od => od.ProductPharmacyPriceListItem)
                    .ThenInclude(ppi => ppi.LifeFileScheduleCode)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
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
                IntegrationTypeId = 1,
                Status = OrderProcessApiTrackingStatusEnum.InProgress,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _clinicDbContext.OrderProcessingApiTrackings.AddAsync(tracking);
            await _clinicDbContext.SaveChangesAsync();

            bool requiresScheduleCode = order.OrderDetails.Any(od => od.ProductPharmacyPriceListItem?.LifeFileScheduledCodeId != null);
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

            var request = order.ToLifeFileOrderRequestDto(configData, requiresScheduleCode, base64String);

            try
            {
                var response = await _client.SendOrderAsync(request);

                var transaction = new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = tracking.Id,
                    Endpoint = "/lfapi/v1/order",
                    Payload = JsonSerializer.Serialize(request),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                if (response?.Type == ResponseTypeEnum.Success)
                {
                    if (response.Data.TryGetProperty("orderId", out var orderIdElement))
                    {
                        var orderIdValue = orderIdElement.ValueKind == JsonValueKind.Number
                            ? orderIdElement.GetInt64().ToString()
                            : orderIdElement.GetString();

                        order.PharmacyOrderNumber = orderIdValue;
                        order.Status = Domain.Enums.OrderStatus.LifeFileSuccess;

                        tracking.Status = OrderProcessApiTrackingStatusEnum.Success;
                        transaction.Status = OrderProcessApiTrackingStatusEnum.Success;
                        transaction.ResponseMessage = $"Order sent successfully. LifeFile OrderId: {orderIdValue}";
                    }
                }
                else if (response?.Type == ResponseTypeEnum.Error)
                {
                    order.Status = Domain.Enums.OrderStatus.LifeFileError;
                    order.IsReadyForLifeFile = false;

                    tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;
                    transaction.Status = OrderProcessApiTrackingStatusEnum.Failed;
                    string errorMessage = response?.Message ?? "Unknown error";
                    if (response?.Data.ValueKind == JsonValueKind.Object)
                    {
                        var errors = new List<string>();
                        foreach (var prop in response.Data.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var msg in prop.Value.EnumerateArray())
                                    errors.Add($"{prop.Name}: {msg.GetString()}");
                            }
                            else
                            {
                                errors.Add($"{prop.Name}: {prop.Value}");
                            }
                        }

                        if (errors.Any())
                            errorMessage = string.Join(" | ", errors);
                    }

                    transaction.ResponseMessage = errorMessage;
                }

                await _clinicDbContext.OrderProcessingApiTransactions.AddAsync(transaction);
                await _clinicDbContext.SaveChangesAsync();
            }
            catch (HttpRequestException ex)
            {
                tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;

                var transaction = new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = tracking.Id,
                    Endpoint = "/lfapi/v1/order",
                    Payload = JsonSerializer.Serialize(request),
                    Status = OrderProcessApiTrackingStatusEnum.Failed,
                    ResponseMessage = $"HTTP request failed: {ex.Message}",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _clinicDbContext.OrderProcessingApiTransactions.AddAsync(transaction);
                await _clinicDbContext.SaveChangesAsync();

                Console.WriteLine($"Failed to send order {orderId}. Error: {ex.Message}");
            }
        }
    }
}
