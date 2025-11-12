using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class PharmacyConfigurationController : BaseApiController<PharmacyConfigurationController>
    {
        private readonly IPharmacyConfigurationService _pharmacyConfigurationService;

        public PharmacyConfigurationController(IPharmacyConfigurationService pharmacyConfigurationService)
        {
            _pharmacyConfigurationService = pharmacyConfigurationService;
        }

        [HttpGet("get-all-active-integrationtypes")]
        public async Task<ActionResult<List<CommonDropDownResponseDto<int>>>> GetActiveIntegrationTypes()
        {
            var result = await _pharmacyConfigurationService.GetActiveIntegrationTypesAsync();
            return Ok(result);
        }

        [HttpGet("integration-keys/{integrationTypeId:int}")]
        public async Task<ActionResult<List<IntegrationKeyResponseDto>>> GetIntegrationKeysByTypeId(int integrationTypeId)
        {
            var result = await _pharmacyConfigurationService.GetIntegrationKeysByTypeIdAsync(integrationTypeId);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult<PharmacyConfigurationResponseDto>> CreatePharmacyConfiguration([FromBody] PharmacyConfigurationRequestDto request)
        {
            if (request is null)
            {
                return BadRequest("Request body cannot be null.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _pharmacyConfigurationService.CreatePharmacyConfigurationAsync(request, userId.Value);

            if (result.PharmacyConfigurationId == Guid.Empty)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("update/{pharmacyConfigurationId:guid}")]
        public async Task<ActionResult<PharmacyConfigurationResponseDto>> UpdatePharmacyConfiguration(
            Guid pharmacyConfigurationId,
            [FromBody] PharmacyConfigurationRequestDto request)
        {
            if (request is null)
            {
                return BadRequest("Request body cannot be null.");
            }
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _pharmacyConfigurationService.UpdatePharmacyConfigurationAsync(
                pharmacyConfigurationId, request, userId.Value);

            if (result.PharmacyConfigurationId == Guid.Empty)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeletePharmacyConfigurations([FromBody] BulkOperationRequestDto<Guid> request)
        {
            if (request.Ids == null || !request.Ids.Any())
            {
                return BadRequest("No pharmacy configuration IDs provided.");
            }
                
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            await _pharmacyConfigurationService.DeletePharmacyConfigurationsAsync(request.Ids);
            return Ok();
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivatePharmacyConfigurations([FromBody] BulkOperationRequestDto<Guid> request)
        {
            if (request?.Ids == null || !request.Ids.Any())
            {
                return BadRequest("No pharmacy configuration IDs provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            await _pharmacyConfigurationService.ActivatePharmacyConfigurationsAsync(request.Ids, userId.Value);
            return Ok();
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivatePharmacyConfigurations([FromBody] BulkOperationRequestDto<Guid> request)
        {
            if (request?.Ids == null || !request.Ids.Any())
            {
                return BadRequest("No pharmacy configuration IDs provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            await _pharmacyConfigurationService.DeactivatePharmacyConfigurationsAsync(request.Ids, userId.Value);
            return Ok();
        }

        [HttpGet("get-all-configurations")]
        public async Task<ActionResult<List<PharmacyConfigurationGetAllResponseDto>>> GetAllConfigurations()
        {
            var result = await _pharmacyConfigurationService.GetAllConfigurationsAsync();
            return Ok(result);
        }

        [HttpGet("get-by-id/{pharmacyConfigurationId:guid}")]
        public async Task<ActionResult<PharmacyConfigurationGetByIdResponseDto>> GetById(Guid pharmacyConfigurationId)
        {
            var result = await _pharmacyConfigurationService.GetConfigurationByIdAsync(pharmacyConfigurationId);
            if (result == null)
                return NotFound("Pharmacy configuration not found.");

            return Ok(result);
        }
    }
}
