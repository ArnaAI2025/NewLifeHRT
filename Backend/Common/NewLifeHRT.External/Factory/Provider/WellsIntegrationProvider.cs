using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.Clients;
using NewLifeHRT.External.Enums;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Mappings;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using System;
using NewLifeHRT.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NewLifeHRT.External.Factory.Provider
{
    public class WellsIntegrationProvider : IIntegrationProvider
    {
        private readonly WellsApiClient _client;
        private readonly ClinicDbContext _clinicDbContext;
        private readonly ITemplateContentGenerator _templateContentGenerator;
        private readonly IPdfConverter _pdfConverter;

        public WellsIntegrationProvider(WellsApiClient client, ClinicDbContext clinicDbContext, ITemplateContentGenerator templateContentGenerator, IPdfConverter pdfConverter)
        {
            _client = client;
            _clinicDbContext = clinicDbContext;
            _templateContentGenerator = templateContentGenerator;
            _pdfConverter = pdfConverter;
        }

        public async Task SendOrderAsync(Guid orderId, Dictionary<string, string> configData)
        {
            try
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
                    .Include(o => o.Physician)
                    .Include(o => o.OrderDetails).ThenInclude(od => od.ProductPharmacyPriceListItem).ThenInclude(ppli => ppli.LifeFileQuantityUnit)
                    .Include(o => o.OrderDetails).ThenInclude(od => od.ProductPharmacyPriceListItem).ThenInclude(ppli => ppli.LifeFileScheduleCode)
                    .Include(o => o.ShippingAddress).ThenInclude(sa => sa.Address).ThenInclude(sa => sa.State)
                    .Include(o => o.PharmacyShippingMethod).ThenInclude(sa => sa.ShippingMethod)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order is null)
                    throw new InvalidOperationException($"Order with ID {orderId} was not found.");

                var tracking = new OrderProcessingApiTracking
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    IntegrationTypeId = 2,
                    Status = OrderProcessApiTrackingStatusEnum.InProgress,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _clinicDbContext.OrderProcessingApiTrackings.AddAsync(tracking);
                await _clinicDbContext.SaveChangesAsync();

                var getTokenRequest = new WellsGetTokenModel.Request { ApiKey = configData["Key"] };
                var response = await _client.GetTokenAsync(getTokenRequest);
                var tokenTransaction = new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = tracking.Id,
                    Endpoint = "/api/Token/Get",
                    Payload = JsonSerializer.Serialize(getTokenRequest),
                    Status = response?.Type == ResponseTypeEnum.Success
                        ? OrderProcessApiTrackingStatusEnum.Success
                        : OrderProcessApiTrackingStatusEnum.Failed,
                    ResponseMessage = response?.Message ?? "No response message",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _clinicDbContext.OrderProcessingApiTransactions.AddAsync(tokenTransaction);

                if (response?.Type == ResponseTypeEnum.Success)
                {
                    var addEditPatientRequest = order.ToWellsAddEditPatientRequestModel(configData);
                    var addEditPatientResponse = await _client.AddEditPatientAsync(addEditPatientRequest);

                    var addEditPatientTransaction = new OrderProcessingApiTransaction
                    {
                        Id = Guid.NewGuid(),
                        OrderProcessingApiTrackingId = tracking.Id,
                        Endpoint = "/api/Patient/AddEditPatient",
                        Payload = JsonSerializer.Serialize(addEditPatientRequest),
                        Status = addEditPatientResponse?.Type == ResponseTypeEnum.Success
                            ? OrderProcessApiTrackingStatusEnum.Success
                            : OrderProcessApiTrackingStatusEnum.Failed,
                        ResponseMessage = addEditPatientResponse?.Message ?? "No response message",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await _clinicDbContext.OrderProcessingApiTransactions.AddAsync(addEditPatientTransaction);

                    if (addEditPatientResponse?.Type == ResponseTypeEnum.Success)
                    {
                        if (!string.IsNullOrEmpty(addEditPatientResponse.Data.PatientID))
                        {
                            if (order.Patient != null)
                            {
                                order.Patient.WellsPatientId = addEditPatientResponse.Data.PatientID;
                                _clinicDbContext.Patients.Update(order.Patient);
                                await _clinicDbContext.SaveChangesAsync();
                            }
                            var status = OrderStatus.LifeFileSuccess;
                            List<string> orderNumbers = new List<string>();
                            foreach (var orderDetail in order.OrderDetails)
                            {
                                var addRxRequest = order.ToWellsAddRxRequestModel(configData, addEditPatientResponse.Data.PatientID, orderDetail);
                                var addRxResponse = await _client.AddRxAsync(addRxRequest);
                                var easyRxTransaction = new OrderProcessingApiTransaction
                                {
                                    Id = Guid.NewGuid(),
                                    OrderProcessingApiTrackingId = tracking.Id,
                                    Endpoint = "/api/Rx/Add",
                                    Payload = JsonSerializer.Serialize(addRxRequest),
                                    Status = addRxResponse?.Type == ResponseTypeEnum.Success
                                        ? OrderProcessApiTrackingStatusEnum.Success
                                        : OrderProcessApiTrackingStatusEnum.Failed,
                                    ResponseMessage = addRxResponse?.Message ?? "No response message",
                                    CreatedAt = DateTime.UtcNow,
                                    CreatedBy = "System"
                                };

                                await _clinicDbContext.OrderProcessingApiTransactions.AddAsync(easyRxTransaction);
                                if (addRxResponse?.Type == ResponseTypeEnum.Success)
                                {
                                    orderNumbers.Add(addRxResponse.Data.RxNumber);
                                }
                                else if (addRxResponse?.Type == ResponseTypeEnum.Error)
                                {
                                    status = OrderStatus.LifeFileError;
                                    order.IsReadyForLifeFile = false;
                                    tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;
                                    Console.WriteLine($"Order {orderId} failed. Error: {response?.Message}");
                                    break;
                                }

                            }
                            order.PharmacyOrderNumber = string.Join(", ", orderNumbers);
                            order.Status = status;

                            if (order.Status == OrderStatus.LifeFileSuccess)
                                tracking.Status = OrderProcessApiTrackingStatusEnum.Success;

                        }
                    }
                    else if (addEditPatientResponse.Type == ResponseTypeEnum.Error)
                    {
                        Console.WriteLine($"Order {orderId} failed. Error: {response?.Message}");
                        order.Status = OrderStatus.LifeFileError;
                        order.IsReadyForLifeFile = false;
                        tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;
                    }

                }
                else if (response?.Type == ResponseTypeEnum.Error)
                {
                    Console.WriteLine($"Order {orderId} failed. Error: {response?.Message}");
                    order.Status = OrderStatus.LifeFileError;
                    order.IsReadyForLifeFile = false;
                    tracking.Status = OrderProcessApiTrackingStatusEnum.Failed;
                }
                await _clinicDbContext.SaveChangesAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to send order {orderId}. Error: {ex.Message}");
            }
        }
    }
}
