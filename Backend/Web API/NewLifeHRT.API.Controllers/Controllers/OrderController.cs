using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Enums;

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
            if (order is null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] OrderRequestDto request)
        {
            if (request is null)
            {
                return BadRequest("Request body is required.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _orderService.CreateOrderAsync(request, userId.Value);
            return Ok(result);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderRequestDto dto)
        {
            if (dto is null)
            {
                return BadRequest("Request body is required.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var proposal = await _orderService.UpdateOrderAsync(id, dto, (int)userId);
            return Ok(proposal);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkOperationRequestDto<Guid> request)
        {
            if (request?.Ids == null || request.Ids.Count == 0)
            {
                return BadRequest("At least one order id must be provided.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _orderService.BulkDeleteAsync(request.Ids);
            return Ok(result);
        }
        [HttpPatch("accept-order/{id}")]
        public async Task<IActionResult> AcceptOrder(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _orderService.UpdateOrderStatusAsync(id, (int)OrderStatus.Completed, null, userId.Value);
            return Ok(result);
        }
        [HttpGet("receipt-by-order/{id}")]
        public async Task<IActionResult> GetReceiptById(Guid id)
        {
            var dto = await _orderService.GetPrescriptionReceiptDataAsync(id, null,true);
            if (dto == null) return NotFound();
            return Ok(dto);
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
            if (request is null)
            {
                return BadRequest("Request body is required.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _orderService.UpdateOrderPaymentAsync(request, userId.Value);
            return Ok(result);
        }
        [HttpPatch("cancel-commission/{id}")]
        public async Task<IActionResult> CancelCommission(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _orderService.CancelGeneratedCommissionAsync(id, userId.Value);
            return Ok(result);
        }

        [HttpPost("refund/{orderId}")]
        public async Task<IActionResult> ProcessRefund(Guid orderId, [FromBody] RefundAmountRequestDto request)
        {
            if (request is null)
            {
                return BadRequest("Request body is required.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _orderService.ProcessRefundAsync(orderId, request.RefundAmount, userId.Value);
            return Ok(result);
        }

        [HttpPost("settle-outstanding-refund/{orderId}")]
        public async Task<IActionResult> SettleOutstandingRefund(Guid orderId, [FromBody] SettleOutstandingRefundRequestDto request)
        {
            if (request is null)
            {
                return BadRequest("Request body is required.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _orderService.SettleOutstandingRefundAsync(orderId, request.SettleAmount, userId.Value);
            return Ok(result);
        }

        [HttpGet("get-all-courier-services")]
        public async Task<IActionResult> GetAllCourierServices()
        {
            var result = await _orderService.GetAllCourierServicesAsync();
            return Ok(result);
        }

        [HttpGet("{orderId}/prescription")]
        public async Task<IActionResult> GetOrderPrescriptionTemplate(Guid orderId, [FromQuery] bool? isScheduleDrug = null)
        {
            var dto = await _orderService.GetPrescriptionReceiptDataAsync(orderId, isScheduleDrug);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // Stream the PDF file for download
        [HttpGet("{orderId}/template/download")]
        public async Task<IActionResult> DownloadOrderPdf(Guid orderId, [FromQuery] bool? isScheduleDrug = null, [FromQuery] bool? isReceipt = null)
        {
            var dto = await _orderService.GetPrescriptionReceiptDataAsync(orderId, isScheduleDrug, isReceipt);
            if (dto == null || string.IsNullOrWhiteSpace(dto.PdfBase64)) return NotFound();

            var pdfBytes = Convert.FromBase64String(dto.PdfBase64);
            var fileName = $"order_{orderId}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

    }
}
