using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Infrastructure.Data;
using System.Threading.Tasks;

namespace MultiTenantTest.Controllers
{
    [ApiController]
    public class PatientController : BaseApiController<PatientController>
    {
        private readonly IPatientService _patientService;
        private readonly IVisitTypeService _visitTypeService;
        private readonly IAgendaService _agendaService;
        private readonly IDocumentCategoryService _documentCategoryService;
        private readonly IPatientCreditCardService _patientCreditCardService;
        private readonly IUserService _userService;
        private readonly ClinicDbContext _context;
        public PatientController(IPatientService patientService, IVisitTypeService visitTypeService, IAgendaService agendaService, IDocumentCategoryService documentCategoryService, IPatientCreditCardService patientCreditCardService, ClinicDbContext context, IUserService userService)
        {
            _patientService = patientService;
            _visitTypeService = visitTypeService;
            _agendaService = agendaService;
            _documentCategoryService = documentCategoryService;
            _patientCreditCardService = patientCreditCardService;
            _context = context;
            _userService = userService;
        }

        [HttpGet("get-all-patients")]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }
        [HttpGet("get-all-active-patients")]
        public async Task<IActionResult> GetAllActivePatients()
        {
            var patients = await _patientService.GetAllActiveAsync();
            return Ok(patients);
        }
        [HttpGet("patients-by-counselor/{counselorId}")]
        public async Task<IActionResult> GetPatientsByCounselorId(int counselorId)
        {
            var patients = await _patientService.GetAllOnCounselorIdAsync(counselorId);
            return Ok(patients);
        }

        [HttpGet("get-credit-cards-by-patient/{patientId}")]
        public async Task<IActionResult> GetCreditCardsByPatientIdAsync(Guid patientId)
        {
            var creditCards = await _patientCreditCardService.GetByPatientIdAsync(patientId);
            return Ok(creditCards);
        }

        [HttpGet("get-all-active-visit-types")]
        public async Task<IActionResult> GetAllActiveVisitType()
        {
            var visitTypes = await _visitTypeService.GetAllAsync();
            return Ok(visitTypes);
        }
        [HttpGet("get-all-active-agendas")]
        public async Task<IActionResult> GetAllActiveAgenda()
        {
            var agendas = await _agendaService.GetAllAsync();
            return Ok(agendas);
        }
        [HttpPatch("activate/{id}")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var userId = GetUserId();
            var response = await _patientService.ToggleActiveStatusAsync(id, userId.Value, true);
            return Ok(response);
        }
        [HttpPatch("deactivate/{id}")]
        public async Task<IActionResult> DeActivate(Guid id)
        {
            var userId = GetUserId();
            var response = await _patientService.ToggleActiveStatusAsync(id, userId.Value, false);
            return Ok(response);
        }
        [HttpPost("create-patient")]
        public async Task<IActionResult> Create([FromForm] CreatePatientRequestDto request)
        {
            var creatorUserId = GetUserId();
            if (creatorUserId == null) return Unauthorized(new CommonOperationResponseDto<Guid?> { Message = "User not authenticated." });

            var result = await _patientService.CreatePatientAndUserAsync(request, creatorUserId.Value);

            if (result.Id == null) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] CreatePatientRequestDto request)
        {
            var updaterUserId = GetUserId();
            if (!updaterUserId.HasValue) return Unauthorized("User not authenticated.");

            var result = await _patientService.UpdatePatientAndUserAsync(id, request, updaterUserId.Value);

            if (result.Id == null) return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("deactivate-bulk")]
        public async Task<IActionResult> DeactivateBulk([FromBody] BulkOperationRequestDto<string> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            var response = await _patientService.BulkTogglePatientStatusAsync(request.Ids, (int)userId.Value, false);
                return Ok(response);
            
        }
        [HttpPatch("activate-bulk")]
        public async Task<IActionResult> ActivateBulk([FromBody] BulkOperationRequestDto<string> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            var response = await _patientService.BulkTogglePatientStatusAsync(request.Ids, (int)userId.Value,true);
                return Ok(response);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeletePatients([FromBody] BulkOperationRequestDto<Guid> request)
        {
            if (request?.Ids == null || !request.Ids.Any())
                return BadRequest("Patient list cannot be empty.");

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            await _patientService.DeletePatientsAndLinkedUsersAsync(request.Ids, userId.Value);
            return Ok(new { Message = $"{request.Ids.Count} patients and linked users successfully deleted." });
        }

        [HttpGet("get-all-active-document-category")]
        public async Task<IActionResult> GetAllDocumentCategory()
        {
            var documentResponseDtos = await _documentCategoryService.GetAllAsync();
            return Ok(documentResponseDtos);
        }
        [HttpPatch("bulk-assign")]
        public async Task<IActionResult> BulkAssignee([FromBody] BulkOperationAssigneeRequestDto<Guid, int> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            if (request == null || request.Ids == null || !request.Ids.Any())
                return BadRequest("Patient IDs are required.");

            if (request.Id == 0)
                return BadRequest("Assignee ID is required.");

            var result = await _patientService.BulkAssignPatientsAsync(request.Ids, request.Id, userId.Value);

            return Ok(result);
        }

        [HttpGet("get-patient-counselor-info")]
        public async Task<IActionResult> GetAllPatientsCounselorInfo()
        {
            var patients = await _patientService.GetAllPatientsCounselorInfo();
            return Ok(patients);
        }
        [HttpGet("get-patient-physician-info/{patientId}")]
        public async Task<IActionResult> GetAllPatientsPhysicianInfo(Guid patientId)
        {
            var patients = await _patientService.GetPhysicianNameByPatientIdAsync(patientId);
            return Ok(patients);
        }

    }
}
