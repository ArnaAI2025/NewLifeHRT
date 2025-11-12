using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class CommissionController : BaseApiController<CommissionController>
    {
        private readonly IPoolDetailService _poolDetailService;
        private readonly ICommissionsPayableService _commissionsPayableService;
        public CommissionController(IPoolDetailService poolDetailService, ICommissionsPayableService commissionsPayableService)
        {
            _poolDetailService = poolDetailService;
            _commissionsPayableService = commissionsPayableService;
        }

        [HttpGet("get-counselors-by-date")]
        public async Task<IActionResult> GetCounselorsByPoolDate([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return BadRequest("Both fromDate and toDate are required.");
            }

            var counselors = await _poolDetailService.GetCounselorsByDateRangeAsync(fromDate, toDate);
            return Ok(counselors);
        }
        [HttpGet("get-commission-by-poolDetailId/{poolDetailId}")]
        public async Task<IActionResult> GetCommissionByPoolDetailId(Guid poolDetailId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var counselors = await _commissionsPayableService.GetCommissionByPoolDetailIdAsync(poolDetailId);
            return Ok(counselors);
        }
        [HttpGet("get-commission-by-getCommissionByIdAsync/{commissionsPayableId}")]
        public async Task<IActionResult> GetCommissionByIdAsync(Guid commissionsPayableId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var counselors = await _commissionsPayableService.GetCommissionByIdAsync(commissionsPayableId);
            return Ok(counselors);
        }
    }
}
