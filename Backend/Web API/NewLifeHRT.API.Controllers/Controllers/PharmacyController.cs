using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class PharmacyController : BaseApiController<PharmacyController>
    {
        private readonly IPharmacyService _pharmacyService;
        private readonly ICurrencyService _currencyService;
        private readonly IShippingMethodService _shippingMethodService;
        private readonly IPharmacyShippingMethodService _pharmacyShippingMethodService;

        public PharmacyController(IPharmacyService pharmacyService, ICurrencyService currencyService, IShippingMethodService shippingMethodService, IPharmacyShippingMethodService pharmacyShippingMethodService)
        {
            _pharmacyService = pharmacyService;
            _currencyService = currencyService;
            _shippingMethodService = shippingMethodService;
            _pharmacyShippingMethodService = pharmacyShippingMethodService;
        }

        [HttpGet("get-all-pharmacy")]
        public async Task<IActionResult> GetAllPharmacy()
        {
            var result = await _pharmacyService.GetAllPharmaciesAsync();
            return Ok(result);
        }

        [HttpGet("get-pharmacy-by-id/{id}")]
        public async Task<IActionResult> GetPharmacyById(Guid id)
        {
            var result = await _pharmacyService.GetPharmacyByIdAsync(id);

            if (result == null)
                return NotFound($"Pharmacy with ID {id} not found.");

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePharmacy([FromBody] PharmacyCreateRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _pharmacyService.CreatePharmacyAsync(request, userId.Value);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePharmacy(Guid id, [FromBody] PharmacyCreateRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var updated = await _pharmacyService.UpdatePharmacyAsync(id, request, userId.Value);
            return Ok(updated);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeletePharmacies([FromBody] PharmacyDeleteRequestDto request)
        {
            if (request.PharmacyIds == null || !request.PharmacyIds.Any())
                return BadRequest("No pharmacy IDs provided.");

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            await _pharmacyService.DeletePharmaciesAsync(request.PharmacyIds, userId.Value);
            return Ok();
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivatePharmacies([FromBody] PharmacyDeleteRequestDto request)
        {
            if (request.PharmacyIds == null || !request.PharmacyIds.Any())
            {
                return BadRequest("No pharmacy IDs provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            await _pharmacyService.ActivatePharmaciesAsync(request.PharmacyIds, userId.Value);
            return Ok();
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivatePharmacies([FromBody] PharmacyDeleteRequestDto request)
        {
            if (request.PharmacyIds == null || !request.PharmacyIds.Any())
            {
                return BadRequest("No pharmacy IDs provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            await _pharmacyService.DeactivatePharmaciesAsync(request.PharmacyIds, userId.Value);
            return Ok();
        }

        [HttpGet("get-all-currencies")]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var result = await _currencyService.GetAllCurrenciesAsync();
            return Ok(result);
        }

        [HttpGet("get-all-pharmacy-for-dropdown")]
        public async Task<IActionResult> GetAllPharmaciesForDropdown()
        {
            var pharmacies = await _pharmacyService.GetAllPharmaciesForDropdownAsync();
            return Ok(pharmacies);
        }
        [HttpGet("get-all-shipping-methods")]
        public async Task<IActionResult> GetAllShippingMethods()
        {
            var pharmacies = await _shippingMethodService.GetAllActiveAsync();
            return Ok(pharmacies);
        }
        [HttpGet("get-all-pharmacy-shipping-methods/{id}")]
        public async Task<IActionResult> GetAllShippingMethods(Guid id)
        {
            var pharmacies = await _pharmacyShippingMethodService.GetAllPharmacyShippingMethod(id);
            return Ok(pharmacies);
        }
        [HttpGet("get-all-active-pharmacies")]
        public async Task<IActionResult> GetAllActivePharmaciesForDropdown()
        {
            var pharmacies = await _pharmacyService.GetAllActivePharmaciesForDropdownAsync();
            return Ok(pharmacies);
        }
    }
}
