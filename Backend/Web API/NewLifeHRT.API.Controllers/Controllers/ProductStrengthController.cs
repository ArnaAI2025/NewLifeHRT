using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class ProductStrengthController : BaseApiController<ProductStrengthController>
    {
        private readonly IProductStrengthService _productStrengthService;
        public ProductStrengthController(IProductStrengthService productStrengthService)
        {
            _productStrengthService = productStrengthService;
        }

        [HttpGet("get-all-strength-by-productid/{productId}")]
        public async Task<IActionResult> GetAllByProductId(Guid productId)
        {
            if (productId == Guid.Empty)
                return BadRequest("Invalid Product ID");

            var result = await _productStrengthService.GetAllByProductIdAsync(productId);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProductStrength([FromBody] ProductStrengthCreateRequestDto request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _productStrengthService.CreateAsync(request, userId.Value);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProductStrength(Guid id, [FromBody] ProductStrengthCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _productStrengthService.UpdateAsync(id, request, userId.Value);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductStrength(Guid id)
        {
            var deletedId = await _productStrengthService.DeleteAsync(id);
            return Ok(deletedId);
        }

    }
}
