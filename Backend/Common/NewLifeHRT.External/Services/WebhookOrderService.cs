using Microsoft.Extensions.Logging;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLifeHRT.External.Interfaces;
using Serilog;
using NewLifeHRT.External.Models;
using System.Linq.Expressions;
using NewLifeHRT.Infrastructure.Helper;

namespace NewLifeHRT.External.Services
{
    public class WebhookOrderService : IWebhookOrderService
    {
        private readonly ILogger<WebhookOrderService> _logger;
        private readonly IOrderProcessingApiTrackingRepository _trackingRepo;
        private readonly IOrderProcessingApiTransactionRepository _trxRepo;
        private readonly IOrderRepository _orderRepository;

        public WebhookOrderService(ILogger<WebhookOrderService> logger, IOrderProcessingApiTrackingRepository trackingRepo, IOrderProcessingApiTransactionRepository trxRepo, IOrderRepository orderRepository)
        {
            _logger = logger;
            _trackingRepo = trackingRepo;
            _trxRepo = trxRepo;
            _orderRepository = orderRepository;
        }

        public Task<WebhookProcessResult> ProcessEipOrderWebhookAsync(string rawPayload)
        {
            return ProcessWebhookAsync<EipOrderWebhookDto>(
                rawPayload,
                idPropertyName: "EipOrderId",
                endpoint: "/webhooks/eip/order",
                getTrackingNumber: dto => dto.ShipmentLines?.FirstOrDefault()?.ShipmentTrackingNumber,
                getCarrierName: dto => dto.ShipmentLines?.FirstOrDefault()?.ShipmentProvider
            );
        }

        public Task<WebhookProcessResult> ProcessLifefileOrderWebhookAsync(string rawPayload)
        {
            return ProcessWebhookAsync<LifefileWebhookDto>(
                rawPayload,
                idPropertyName: "orderId",
                endpoint: "/webhooks/lifefile/order",
                getTrackingNumber: dto => dto.TrackingNumber,
                getCarrierName: dto => dto.ShipCarrier
            );
        }

        public async Task<WebhookProcessResult> ProcessWellsOrderWebhookAsync(string rawPayload)
        {
            bool isArrayWrapper = rawPayload.Contains("\"events\"");

            if (isArrayWrapper)
            {
                var root = JsonHelper.DeserializeSafe<WellsWebhookRootDto>(rawPayload);

                foreach (var evt in root.Events)
                {
                    var eachJson = JsonConvert.SerializeObject(evt);

                    var result = await ProcessWebhookAsync<WellsWebhookDto>(
                        eachJson,
                        idPropertyName: "wpn_order_nbr",
                        endpoint: "/webhooks/wells/order",
                        getTrackingNumber: dto => dto?.Data?.TrackingNbr,
                        getCarrierName: dto => dto?.Data?.Carrier
                    );

                    if (!result.Success)
                        return result;
                }

                return new WebhookProcessResult
                {
                    Success = true,
                    Message = "All wells webhook events processed.",
                    HttpStatusCode = 200
                };
            }

            return await ProcessWebhookAsync<WellsWebhookDto>(
                rawPayload,
                idPropertyName: "wpn_order_nbr",
                endpoint: "/webhooks/wells/order",
                getTrackingNumber: dto => dto?.Data?.TrackingNbr,
                getCarrierName: dto => dto?.Data?.Carrier
            );
        }

        private async Task<WebhookProcessResult> ProcessWebhookAsync<TDto>(string rawPayload, string idPropertyName, string endpoint, Func<TDto, string> getTrackingNumber, Func<TDto, string> getCarrierName)
        {
            Guid trackingId = Guid.NewGuid();
            OrderProcessingApiTracking tracking = null;

            string orderId = JsonHelper.ExtractPropertySafe(rawPayload, idPropertyName);

            if (orderId == null)
            {
                _logger.LogError("{Prop} not found in webhook payload.", idPropertyName);
                return new WebhookProcessResult
                {
                    Success = false,
                    Message = $"{idPropertyName} not found in payload",
                    HttpStatusCode = 400
                };
            }

            var order = (await _orderRepository.FindWithIncludeAsync(
                new List<Expression<Func<Order, bool>>> { a => a.PharmacyOrderNumber == orderId },
                new[] { "Pharmacy.Configuration.IntegrationType" }
            )).FirstOrDefault();

            if (order == null)
            {
                _logger.LogError("Order not found for ID {OrderId}.", orderId);
                return new WebhookProcessResult
                {
                    Success = false,
                    Message = "Order not found",
                    HttpStatusCode = 404
                };
            }

            try
            {
                var model = JsonHelper.DeserializeSafe<TDto>(rawPayload);
                tracking = new OrderProcessingApiTracking
                {
                    Id = trackingId,
                    OrderId = order.Id,
                    IntegrationTypeId = order.Pharmacy.Configuration.TypeId,
                    Status = OrderProcessingApiTrackingStatusEnum.InProgress,
                    IsFromWebhook = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Webhook"
                };

                await _trackingRepo.AddAsync(tracking);

                string carrierName = getCarrierName(model);
                // Update tracking number
                string trackingNumber = getTrackingNumber(model);
                if (!string.IsNullOrWhiteSpace(trackingNumber))
                {
                    order.TrackingNumber = trackingNumber;
                    await _orderRepository.UpdateAsync(order);
                }

                // Mark success
                tracking.Status = OrderProcessingApiTrackingStatusEnum.Success;
                await _trackingRepo.UpdateAsync(tracking);

                await _trxRepo.AddAsync(new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = trackingId,
                    Endpoint = endpoint,
                    Payload = rawPayload,
                    Status = OrderProcessingApiTrackingStatusEnum.Success,
                    ResponseMessage = "Webhook processed successfully",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Webhook"
                });

                return new WebhookProcessResult
                {
                    Success = true,
                    Message = "Webhook processed",
                    HttpStatusCode = 200
                };
            }
            catch (Exception ex)
            {
                if (tracking == null)
                {
                    tracking = new OrderProcessingApiTracking
                    {
                        Id = trackingId,
                        OrderId = order.Id,
                        IntegrationTypeId = order.Pharmacy.Configuration.TypeId,
                        Status = OrderProcessingApiTrackingStatusEnum.Failed,
                        IsFromWebhook = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "Webhook"
                    };

                    await _trackingRepo.AddAsync(tracking);
                }
                else
                {
                    tracking.Status = OrderProcessingApiTrackingStatusEnum.Failed;
                    await _trackingRepo.UpdateAsync(tracking);
                }

                await _trxRepo.AddAsync(new OrderProcessingApiTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderProcessingApiTrackingId = trackingId,
                    Endpoint = endpoint,
                    Payload = rawPayload,
                    Status = OrderProcessingApiTrackingStatusEnum.Failed,
                    ResponseMessage = ex.Message,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Webhook"
                });
                return new WebhookProcessResult
                {
                    Success = false,
                    Message = ex.Message,
                    HttpStatusCode = 500
                };
            }
        }

    }
}
