using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTenantTest.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class CommisionRateController : BaseApiController<CommisionRateController>
    {
        private readonly ICommisionRateService _commisionRateService;
        public CommisionRateController(ICommisionRateService commisionRateService)
        {
            _commisionRateService = commisionRateService;
        }

        [HttpGet("get-all-commissionrate")]
        public async Task<IActionResult> GetAllCommissionRate()
        {
            var commissionRates = await _commisionRateService.GetAllCommisionRatesAsync();
            return Ok(commissionRates);
        }

        [HttpGet("get-commissionrate-by-id/{id}")]
        public async Task<IActionResult> GetCommissionRateById(Guid id)
        {
            var commissionRate = await _commisionRateService.GetCommisionRateByIdAsync(id);
            if (commissionRate == null)
                return NotFound();
            return Ok(commissionRate);
        }

        [HttpGet("get-commissionrate-by-productId/{productId}")]
        public async Task<IActionResult> GetCommissionRateByProductId(Guid productId)
        {
            var commissionRates = await _commisionRateService.GetCommisionRateByProductIdAsync(productId);
            return Ok(commissionRates);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCommissionRate([FromBody] CommisionRateRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _commisionRateService.CreateCommisionRateAsync(dto, userId.Value);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCommissionRate(Guid id, [FromBody] CommisionRateRequestDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var response = await _commisionRateService.UpdateCommisionRateAsync(id, dto, userId.Value);
            return Ok(response);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCommissionRate([FromBody] List<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Ids list cannot be empty.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            await _commisionRateService.DeleteCommisionRateAsync(ids, userId.Value);
            return Ok();
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateCommissionRate([FromBody] List<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Ids list cannot be empty.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            await _commisionRateService.ActivateCommisionRateAsync(ids, userId.Value);
            return Ok();
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateCommissionRate([FromBody] List<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Ids list cannot be empty.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            await _commisionRateService.DeactivateCommisionRateAsync(ids, userId.Value);
            return Ok();
        }
    }
}