using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class OrderProductsRefillController : BaseApiController<OrderProductsRefillController>
    {
        private readonly IOrderProductsRefillService _orderProductsRefillService;

        public OrderProductsRefillController(IOrderProductsRefillService orderProductsRefillService)
        {
            _orderProductsRefillService = orderProductsRefillService;
        }


        [HttpGet("getAllOrderProductsRefill")]
        public async Task<IActionResult> GetAllOrderProductsRefill()
        {

            var result = await _orderProductsRefillService.GetAllOrderProductRefillAsync();
            return Ok(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteOrderProductRefillRecords([FromBody] List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("No record IDs provided.");

            var result = await _orderProductsRefillService.DeleteOrderProductRefillRecordsAsync(ids);
            return Ok(new { message = $"{result} record(s) deleted successfully." });
        }

        [HttpGet("getOrderProductRefillById/{id}")]
        public async Task<IActionResult> GetOrderProductRefillById([FromRoute] Guid id)
        {
            var result = await _orderProductsRefillService.GetOrderProductRefillByIdAsync(id);

            if (result == null)
                return NotFound($"OrderProductRefillDetail with Id '{id}' not found.");

            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrderProductRefillDetail([FromRoute] Guid id, [FromBody] UpdateOrderProductRefillDetailRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request data.");

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _orderProductsRefillService.UpdateOrderProductRefillDetailAsync(id, request, userId.Value);

            if (!result)
                return NotFound($"OrderProductRefillDetail with Id '{id}' not found.");

            return Ok(new { message = "Order Product Refill Detail updated successfully." });

        }

    }
}
