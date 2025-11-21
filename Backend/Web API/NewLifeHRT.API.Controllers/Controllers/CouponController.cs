using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class CouponController : BaseApiController<CouponController>
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var coupons = await _couponService.GetAllAsync();
            return Ok(coupons);
        }

        [HttpGet("get-coupon-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var coupon = await _couponService.GetCouponById(id);
            return Ok(coupon);
        }
        [HttpGet("get-active-coupons")]
        public async Task<IActionResult> GetActiveCoupons()
        {
            var coupon = await _couponService.GetActiveCoupons();
            return Ok(coupon);
        }
        [HttpGet("get-coupons")]
        public async Task<IActionResult> GetCoupons()
        {
            var coupon = await _couponService.GetCoupons();
            return Ok(coupon);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CouponRequestDto request)
        {
            var userId = GetUserId();
            var response = await _couponService.Create(request, userId.Value);
            return Ok(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CouponRequestDto request)
        {
            var userId = GetUserId();
            var response = await _couponService.Update(id, request, userId.Value);
            return Ok(response);
        }

        [HttpPatch("bulk-toggle-active")]
        public async Task<IActionResult> BulkToggleActiveStatus([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            var response = await _couponService.BulkToggleActiveStatusAsync(request.Ids, userId.Value, true);
            return Ok(response);
        }
        [HttpPatch("bulk-toggle-inactive")]
        public async Task<IActionResult> BulkToggleInActiveStatus([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            var response = await _couponService.BulkToggleActiveStatusAsync(request.Ids, userId.Value, false);
            return Ok(response);
        }
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var response = await _couponService.BulkDeleteProposalAsync(request.Ids);
            return Ok(response);
        }
    }
}
