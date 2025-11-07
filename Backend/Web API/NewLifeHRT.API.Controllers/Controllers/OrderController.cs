using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Api.Requests;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using System.Runtime.CompilerServices;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class OrderController : BaseApiController<OrderController>
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingApiTrackingService _orderProcessingApiTrackingService;
        public OrderController(IOrderService orderService, IOrderProcessingApiTrackingService orderProcessingApiTrackingService)
        {
            _orderService = orderService;
            _orderProcessingApiTrackingService = orderProcessingApiTrackingService;
        }
        [HttpGet("get-all/{patientId?}")]
        public async Task<IActionResult> GetAll(Guid? patientId)
        {
            var orders = await _orderService.GetAllAsync(patientId);
            return Ok(orders);
        }
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] OrderRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.CreateOrderAsync(request, userId.Value);
            return Ok(result);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderRequestDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var proposal = await _orderService.UpdateOrderAsync(id, dto, (int)userId);
            return Ok(proposal);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.BulkDeleteAsync(request.Ids);
            return Ok(result);
        }
        [HttpPatch("accept-order/{id}")]
        public async Task<IActionResult> AcceptOrder(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.UpdateOrderStatusAsync(id, (int)OrderStatus.Completed, null, userId.Value);

            return Ok(result);
        }
        [HttpGet("receipt-by-order/{id}")]
        public async Task<IActionResult> GetReceiptById(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var order = await _orderService.GetReceiptByIdAsync(id);
            return Ok(order);
        }
        [HttpGet("prescription-by-order/{id}")]
        public async Task<IActionResult> GetFullOrderById(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var order = await _orderService.GetFullOrderByIdAsync(id,null);
            return Ok(order);
        }
        [HttpGet("signed-prescription-by-order/{id}")]
        public async Task<IActionResult> GetSignedOrderById(Guid id, bool? isSigned)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var order = await _orderService.GetFullOrderByIdAsync(id, isSigned);
            return Ok(order);
        }

        [HttpPatch("mark-ready-to-lifefile/{orderId}")]
        public async Task<IActionResult> MarkReadyForLifeFile(Guid orderId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.MarkOrderReadyForLifeFileAsync(orderId, userId.Value);
            return Ok(result);
        }
        [HttpPatch("generate-commission/{orderId}")]
        public async Task<IActionResult> GenerateCommission(Guid orderId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.GenerateCommission(orderId, userId.Value);
            return Ok(result);
        }

        [HttpGet("get-all-orderprocessing-api-tracking-errors")]
        public async Task<IActionResult> GetErrorTrackings()
        {
            var result = await _orderProcessingApiTrackingService.GetErrorTrackingsAsync();
            return Ok(result);
        }

        [HttpPatch("update-payment")]
        public async Task<IActionResult> UpdateOrderPayment([FromBody] UpdateOrderPaymentRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.UpdateOrderPaymentAsync(request, userId.Value);
            return Ok(result);
        }
        [HttpPatch("cancel-commission/{id}")]
        public async Task<IActionResult> CancelCommission(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.CancelGeneratedCommissionAsync(id, userId.Value);
            return Ok(result);
        }

        [HttpPost("refund/{orderId}")]
        public async Task<IActionResult> ProcessRefund(Guid orderId, [FromBody] RefundAmountRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.ProcessRefundAsync(orderId, request.RefundAmount, userId.Value);
            return Ok(result);
        }

        [HttpPost("settle-outstanding-refund/{orderId}")]
        public async Task<IActionResult> SettleOutstandingRefund(Guid orderId, [FromBody] SettleOutstandingRefundRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.SettleOutstandingRefundAsync(orderId, request.SettleAmount, userId.Value);
            return Ok(result);
        }

    }
}
