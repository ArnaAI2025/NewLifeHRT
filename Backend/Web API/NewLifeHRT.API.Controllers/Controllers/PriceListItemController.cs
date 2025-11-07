using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class PriceListItemController : BaseApiController<PriceListItemController>
    {
        private readonly IPriceListItemService _priceListItemService;
        private readonly ILifeFileScheduleCodeService _lifeFileScheduleCodeService;
        private readonly ILifeFileDrugFormService _lifeFileDrugFormService;
        private readonly ILifeFileQuantityUnitsService _lifeFileQuantityUnitsService;

        public PriceListItemController(IPriceListItemService priceListItemService, ILifeFileScheduleCodeService lifeFileScheduleCodeService, ILifeFileDrugFormService lifeFileDrugFormService, ILifeFileQuantityUnitsService lifeFileQuantityUnitsService)
        {
            _priceListItemService = priceListItemService;
            _lifeFileScheduleCodeService = lifeFileScheduleCodeService;
            _lifeFileDrugFormService = lifeFileDrugFormService;
            _lifeFileQuantityUnitsService = lifeFileQuantityUnitsService;
        }

        [HttpGet("get-all-pricelistitems")]
        public async Task<IActionResult> GetAllPriceListItems()
        {
            var result = await _priceListItemService.GetAllPriceListItemsAsync();
            return Ok(result);
        }

        [HttpGet("get-pricelistitem-by-id/{id}")]
        public async Task<IActionResult> GetPriceListItemById(Guid id)
        {
            var result = await _priceListItemService.GetPriceListItemByIdAsync(id);

            if (result == null)
                return NotFound($"Price List Item with ID {id} not found.");

            return Ok(result);
        }

        [HttpGet("get-pricelistitem-by-productid/{productid}")]
        public async Task<IActionResult> GetPriceListItemByProductId(Guid productid)
        {
            var result = await _priceListItemService.GetPriceListItemByProductIdAsync(productid);

            if (result == null)
                return NotFound($"Price List Item with Product ID {productid} not found.");

            return Ok(result);
        }

        [HttpGet("get-pricelistitem-by-pharmacyid/{pharmacyid}")]
        public async Task<IActionResult> GetPriceListItemByPharmacyId(Guid pharmacyid)
        {
            var result = await _priceListItemService.GetPriceListItemByPharmacyIdAsync(pharmacyid, null);

            if (result == null)
                return NotFound($"Price List Item with Pharmacy ID {pharmacyid} not found.");

            return Ok(result);
        }
        [HttpGet("get-active-pricelistitem-by-pharmacyid/{pharmacyid}")]
        public async Task<IActionResult> GetActivePriceListItemByPharmacyId(Guid pharmacyid)
        {
            var result = await _priceListItemService.GetPriceListItemByPharmacyIdAsync(pharmacyid, true);

            if (result == null || !result.Any())
                return NotFound($"No active price list items found for Pharmacy ID {pharmacyid}.");

            return Ok(result);
        }

        [HttpGet("get-active-pricelist-item-by-pharmacyid/{pharmacyid}")]
        public async Task<IActionResult> GetActiveProductPriceListItemByPharmacyId(Guid pharmacyid)
        {
            var result = await _priceListItemService.GetActiveProductsPriceListItemByPharmacyIdAsync(pharmacyid, true);

            if (result == null || !result.Any())
                return NotFound($"No active price list items found for Pharmacy ID {pharmacyid}.");

            return Ok(result);
        }


        [HttpGet("get-all-lifefiledrugforms")]
        public async Task<IActionResult> GetAllLifeFileDrugForms()
        {
            var result = await _lifeFileDrugFormService.GetAllLifeFileDrugFormsAsync();
            return Ok(result);
        }

        [HttpGet("get-all-lifefilequantityunits")]
        public async Task<IActionResult> GetAllLifeFileQuantityUnits()
        {
            var result = await _lifeFileQuantityUnitsService.GetAllLifeFileQuantityUnitsAsync();
            return Ok(result);
        }

        [HttpGet("get-all-lifefileschedulecodes")]
        public async Task<IActionResult> GetAllLifeFileScheduleCodes()
        {
            var result = await _lifeFileScheduleCodeService.GetAllLifeFileScheduleCodesAsync();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePriceListItem([FromBody] PriceListItemRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _priceListItemService.CreatePriceListItemAsync(request, userId.Value);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePriceListItem(Guid id, [FromBody] PriceListItemRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var updated = await _priceListItemService.UpdatePriceListItemAsync(id, request, userId.Value);
            return Ok(updated);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeletePriceListItem([FromBody] PriceListItemActionsRequestDto request)
        {
            if (request.PriceListItemIds == null || !request.PriceListItemIds.Any())
                return BadRequest("No Price List Item IDs provided.");

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            await _priceListItemService.DeletePriceListItemAsync(request.PriceListItemIds, userId.Value);
            return Ok();
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivatePriceListItem([FromBody] PriceListItemActionsRequestDto request)
        {
            if (request.PriceListItemIds == null || !request.PriceListItemIds.Any())
            {
                return BadRequest("No Price List Item IDs provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            await _priceListItemService.ActivatePriceListItemAsync(request.PriceListItemIds, userId.Value);
            return Ok();
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivatePharmacies([FromBody] PriceListItemActionsRequestDto request)
        {
            if (request.PriceListItemIds == null || !request.PriceListItemIds.Any())
            {
                return BadRequest("No Price List Item IDs provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            await _priceListItemService.DeactivatePriceListItemAsync(request.PriceListItemIds, userId.Value);
            return Ok();
        }


    }
}
