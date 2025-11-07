using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class ReminderController : BaseApiController<ReminderController>
    {
        private readonly IReminderService _reminderService;
        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetRemindersByPatientId(Guid patientId)
        {
            var response = await _reminderService.GetRemindersByPatientIdAsync(patientId);
            return Ok(response);
        }

        [HttpGet("lead/{leadId}")]
        public async Task<IActionResult> GetRemindersByLeadId(Guid leadId)
        {
            var response = await _reminderService.GetRemindersByLeadIdAsync(leadId);
            return Ok(response);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetReminderTypes()
        {
            var response = await _reminderService.GetAllReminderTypesAsync();
            return Ok(response);
        }

        [HttpGet("recurrence-rules")]
        public async Task<IActionResult> GetRecurrenceRules()
        {
            var response = await _reminderService.GetAllRecurrenceRulesAsync();
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReminder([FromBody] CreateReminderRequestDto request)
        {
            if (request.PatientId == null && request.LeadId == null)
            {
                return BadRequest("Either PatientId or LeadId must be provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            var response = await _reminderService.CreateReminderAsync(request, userId.Value);
            return Ok(response);
        }

        [HttpGet("patient/all")]
        public async Task<IActionResult> GetAllActiveRemindersForPatients()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var response = await _reminderService.GetTodayRemindersForAllPatientsAsync(userId.Value);
            return Ok(response);
        }

        [HttpGet("lead/all")]
        public async Task<IActionResult> GetAllActiveRemindersForLeads()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var response = await _reminderService.GetTodayRemindersForAllLeadsAsync(userId.Value);
            return Ok(response);
        }

        [HttpPut("mark-completed/{reminderId}")]
        public async Task<IActionResult> MarkReminderAsCompleted(Guid reminderId)
        {
            var response = await _reminderService.MarkReminderAsCompletedAsync(reminderId);

            if (response.Id == Guid.Empty)
                return NotFound(response.Message);

            return Ok(response);
        }
    }
}
