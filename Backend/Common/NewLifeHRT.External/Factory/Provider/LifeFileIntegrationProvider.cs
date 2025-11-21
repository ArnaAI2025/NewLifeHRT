using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.External.Clients;
using NewLifeHRT.External.Enums;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Mappings;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Interfaces;
using NewLifeHRT.Infrastructure.Models.RefillCalculation;
using NewLifeHRT.Infrastructure.Settings;
using System.Text.Json;

namespace NewLifeHRT.External.Factory.Provider
{
    public class LifeFileIntegrationProvider : IIntegrationProvider
    {
        private readonly LifeFileApiClient _client;
        private readonly ClinicDbContext _clinicDbContext;
        private readonly ITemplateContentGenerator _templateContentGenerator;
        private readonly IPdfConverter _pdfConverter;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;
        private readonly IRefillDateCalculator _calculator;
        private readonly ILogger<LifeFileIntegrationProvider> _logger;

        public LifeFileIntegrationProvider(LifeFileApiClient client, ClinicDbContext clinicDbContext, ITemplateContentGenerator templateContentGenerator, IPdfConverter pdfConverter, IOptions<AzureBlobStorageSettings> azureBlobStorageSettings, IRefillDateCalculator calculator, ILogger<LifeFileIntegrationProvider> logger)
        {
            _client = client;
            _clinicDbContext = clinicDbContext;
            _templateContentGenerator = templateContentGenerator;
            _pdfConverter = pdfConverter;
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
            _calculator = calculator;
            _logger = logger;
        }

        /// <summary>
        /// Sends an order to the LifeFile API for processing.
        /// Handles tracking, retries for failed attempts, 
        /// PDF generation for controlled drugs, and response logging.
        /// </summary>
        public async Task SendOrderAsync(Guid orderId, Dictionary<string, string> configData, string type)
        {
            _client.Initialize(configData,type);

            var failedTrackings = await _clinicDbContext.OrderProcessingApiTrackings
                .Where(t => t.OrderId == orderId && t.Status == OrderProcessingApiTrackingStatusEnum.Failed && !t.IsFromWebhook)
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
                .Include(o => o.Pharmacy).ThenInclude(sa => sa.Configuration).ThenInclude(c=>c.IntegrationType)
                .Include(o => o.Physician).ThenInclude(p => p.UserSignatures)
                .FirstOrDefaultAsync(o => o.Id == orderId);


            if (order is null)
                throw new InvalidOperationException($"Order with ID {orderId} was not found.");

            var tracking = new OrderProcessingApiTracking
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                IntegrationTypeId = order.Pharmacy.Configuration.IntegrationType.Id,
                Status = OrderProcessingApiTrackingStatusEnum.InProgress,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _clinicDbContext.OrderProcessingApiTrackings.AddAsync(tracking);
            await _clinicDbContext.SaveChangesAsync();

            bool requiresScheduleCode = order.OrderDetails.Any(od => od.ProductPharmacyPriceListItem?.LifeFileScheduledCodeId != null);
            string? base64String = null;
            var refillDict = new Dictionary<Guid, RefillResultModel>();
            if (requiresScheduleCode)
            {

                foreach (var od in order.OrderDetails)
                {
                    var input = new RefillInputModel
                    {
                        ProductName = od.Product?.Name ?? od.ProductPharmacyPriceListItem?.LifeFileDrugName ?? "",
                        Protocol = od.Protocol ?? "",
                        Quantity = od.Quantity,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    };

                    var refillResult = await _calculator.CalculateAsync(input);
                    refillDict[od.Id] = refillResult;
                }

                var model = order.ToControlledDrugOrderModel(configData, _azureBlobStorageSettings, refillDict);
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

            object? request = null;
            try
            {
                try
                {
                    if (type.Equals("aps", StringComparison.OrdinalIgnoreCase))
                    {
                        request = order.ToAPSOrderRequestDto(configData, requiresScheduleCode, base64String, refillDict);
                    }
                    else
                    {
                        request = order.ToLifeFileOrderRequestDto(configData, requiresScheduleCode, base64String, refillDict);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e,$"Error while creating request: {e.Message}");
                }

                var response = await _client.SendOrderAsync(request);

                var transaction = new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = tracking.Id,
                    Endpoint = "/lfapi/v1/order",
                    Payload = JsonSerializer.Serialize(request,LifeFileApiClient._jsonOptions),
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

                        tracking.Status = OrderProcessingApiTrackingStatusEnum.Success;
                        transaction.Status = OrderProcessingApiTrackingStatusEnum.Success;
                        transaction.ResponseMessage = $"Order sent successfully. LifeFile OrderId: {orderIdValue}";
                    }
                }
                else if (response?.Type == ResponseTypeEnum.Error)
                {
                    order.Status = Domain.Enums.OrderStatus.LifeFileError;
                    order.IsReadyForLifeFile = false;

                    tracking.Status = OrderProcessingApiTrackingStatusEnum.Failed;
                    transaction.Status = OrderProcessingApiTrackingStatusEnum.Failed;
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
                tracking.Status = OrderProcessingApiTrackingStatusEnum.Failed;

                var transaction = new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = tracking.Id,
                    Endpoint = "/lfapi/v1/order",
                    Payload = JsonSerializer.Serialize(request),
                    Status = OrderProcessingApiTrackingStatusEnum.Failed,
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
