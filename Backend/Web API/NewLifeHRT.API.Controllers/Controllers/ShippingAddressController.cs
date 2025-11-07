using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using NewLifeHRT.Application.Services.Services;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class ShippingAddressController : BaseApiController<CouponController>
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;

        public ShippingAddressController(IShippingAddressService shippingAddressService, ICountryService countryService, IStateService stateService)
        {
            _shippingAddressService = shippingAddressService;
            _countryService = countryService;
            _stateService = stateService;
        }

        [HttpGet("get-all/{id}")]
        public async Task<IActionResult> GetAllAsync(Guid id)
        {
            var coupons = await _shippingAddressService.GetAllAsync(id);
            return Ok(coupons);
        }

        [HttpGet("get-shipping-address-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var coupon = await _shippingAddressService.GetAllByIdAsync(id);
            return Ok(coupon);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ShippingAddressRequestDto request)
        {
            var userId = GetUserId();
            var response = await _shippingAddressService.CreateAsync(request, userId.Value,null, false);
            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ShippingAddressRequestDto request)
        {
            var userId = GetUserId();
            var response = await _shippingAddressService.UpdateAsync(request, userId.Value);
            return Ok(response);
        }

        [HttpPatch("bulk-toggle-active")]
        public async Task<IActionResult> BulkToggleActiveStatus([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            var response = await _shippingAddressService.BulkToggleActiveAsync(request.Ids, userId.Value, true);
            return Ok(response);
        }
        [HttpPatch("bulk-toggle-inactive")]
        public async Task<IActionResult> BulkToggleInActiveStatus([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            var response = await _shippingAddressService.BulkToggleActiveAsync(request.Ids, userId.Value, false);
            return Ok(response);
        }
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var response = await _shippingAddressService.BulkDeleteAsync(request.Ids);
            return Ok(response);
        }
        [HttpGet("get-all-active-countries")]
        public async Task<IActionResult> GetAllActiveCountries()
        {
            var counties = await _countryService.GetAllAsync();
            return Ok(counties);
        }
        [HttpGet("get-all-active-states-by-countryId/{countryId}")]
        public async Task<IActionResult> GetAllActiveStates(int countryId)
        {
            var states = await _stateService.GetAllAsync(countryId);
            return Ok(states);
        }
    }
}
