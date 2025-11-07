using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.DTOs.Leads;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Enums;
using System;
using System.Threading.Tasks;
using MultiTenantTest.Controllers;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Models.Response;

namespace NewLifeHRT.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class LeadController : BaseApiController<LeadController>
    {
        private readonly ILeadService _leadService;
        private readonly IPatientService _patientService;

        public LeadController(ILeadService leadService, IPatientService patientService)
        {
            _leadService = leadService;
            _patientService = patientService;
        }

        [HttpGet("get-all-leads")]
        public async Task<IActionResult> GetAll([FromQuery] int? ownerId = null)
        {
            var leads = await _leadService.GetAllAsync(ownerId);
            return Ok(leads);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var lead = await _leadService.GetByIdAsync(id);
            if (lead == null)
                return NotFound();

            return Ok(lead);
        }
        [HttpGet("leads-by-counselor/{counselorId}")]
        public async Task<IActionResult> GetPatientsByCounselorId(int counselorId)
        {
            var patients = await _leadService.GetAllOnCounselorIdAsync(counselorId);
            return Ok(patients);
        }

        [HttpPost("create-lead")]
        public async Task<IActionResult> Create([FromBody] LeadRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _leadService.CreateAsync(request, userId.Value);
            return Ok(response);
        }

        [HttpPut("update-lead/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LeadRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _leadService.UpdateAsync(id, request, userId.Value);
            return Ok(response);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteLeads([FromBody] BulkOperationRequestDto<Guid> request)
        {
            if (request?.Ids == null || !request.Ids.Any())
            {
                return BadRequest("LeadIds list cannot be empty.");
            }
            var userId = GetUserId();
            await _leadService.BulkDeleteLeadsAsync(request.Ids, userId.Value);
            return Ok();
        }

        [HttpPatch("bulk-toggle-active")]
        public async Task<IActionResult> BulkToggleActive([FromBody] BulkOperationRequestDto<Guid> request)
        {            
           var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
                     
           var result = await _leadService.BulkToggleActiveStatusAsync(request.Ids, (int)userId, true);           
            return Ok(result);
        }
        [HttpPatch("bulk-toggle-inActive")]
        public async Task<IActionResult> BulkToggleInActive([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _leadService.BulkToggleActiveStatusAsync(request.Ids, (int)userId, false);
            return Ok(result);
        }
        [HttpPatch("bulk-assign")]
        public async Task<IActionResult> BulkAssignee([FromBody] BulkOperationAssigneeRequestDto<Guid, int> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            if (request == null || request.Ids == null || !request.Ids.Any())
                return BadRequest("Lead IDs are required.");

            if (request.Id == 0)
                return BadRequest("Assignee ID is required.");

            var result = await _leadService.BulkAssignLeadsAsync(request.Ids, request.Id, userId.Value);

            return Ok(result);
        }
        [HttpPatch("convert-to-patient")]
        public async Task<IActionResult> ConvertToPatientAsync(BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var leads = await _leadService.ConvertToPatientRequestAsync(request.Ids);
            var result = await _patientService.CreateMultipleAsync(leads, userId.Value);
            if (result.Id == null || !result.Id.Any())
            {
                return BadRequest(new
                {
                    Message = "Failed to convert leads to patients. No patients were created.",
                    Details = result.Message
                });
            }

            var status = await _leadService.BulkToggleIsQualifiedAsync(request.Ids, true, userId.Value);

            return Ok(new
            {
                PatientCreationMessage = result.Message,
                LeadQualificationMessage = status.Message,
                CreatedPatientIds = result.Id
            });
        }
        [HttpPatch("disqualify-lead")]
        public async Task<IActionResult> DisQualifyLeadAsync(BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var status = await _leadService.BulkToggleIsQualifiedAsync(request.Ids, false, userId.Value);
            return Ok(status);
        }

    }
}
