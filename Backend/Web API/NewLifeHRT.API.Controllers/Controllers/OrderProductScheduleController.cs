using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class OrderProductScheduleController : BaseApiController<OrderProductScheduleController>
    {
        private readonly IOrderProductScheduleService _orderProductScheduleService;

        public OrderProductScheduleController(IOrderProductScheduleService orderProductScheduleService)
        {
            _orderProductScheduleService = orderProductScheduleService;
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetSchedules([FromBody] OrderProductScheduleFilterRequestDto request)
        {
            if (request is null)
            {
                return BadRequest(new
                {
                    isPatient = false,
                    message = "Invalid request body supplied."
                });
            }

            if (request.EndDate < request.StartDate)
            {
                return BadRequest(new
                {
                    isPatient = false,
                    message = "End date must be on or after start date."
                });
            }
            var patientId = GetLoggedInPatientId();
            if (patientId == null)
            {
                return Ok(new
                {
                    isPatient = false,
                    message = "The logged-in user is not a patient.",
                    schedules = new List<OrderProductScheduleResponseDto>()
                });
            }

            var schedules = await _orderProductScheduleService.GetSchedulesForLoggedInPatientAsync(
                patientId.Value,
                request.StartDate,
                request.EndDate);

            return Ok(new
            {
                isPatient = true,
                message = "Success",
                schedules
            });
        }

        [HttpGet("patient/schedules/summary")]
        public async Task<IActionResult> GetLoggedInPatientScheduleSummary()
        {
            var patientId = GetLoggedInPatientId();
            if (patientId == null)
            {
                return Ok(new
                {
                    isPatient = false,
                    message = "The logged-in user is not a patient.",
                    schedules = new List<OrderProductScheduleSummaryResponseDto>()
                });
            }

            var summaries = await _orderProductScheduleService.GetScheduleSummaryForPatientAsync(patientId.Value);

            return Ok(new
            {
                isPatient = true,
                message = "Success",
                schedules = summaries
            });
        }

        [HttpGet("summary/{id:guid}")]
        public async Task<IActionResult> GetScheduleSummaryById(Guid id)
        {
            var result = await _orderProductScheduleService.GetScheduleSummaryByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Schedule summary not found." });

            return Ok(new
            {
                message = "Success",
                schedule = result
            });
        }

        [HttpPut("summary/{id:guid}")]
        public async Task<IActionResult> UpdateScheduleSummary(Guid id, [FromBody] UpdateOrderProductScheduleSummaryRequestDto request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request body." });

            var success = await _orderProductScheduleService.UpdateScheduleSummaryAsync(id, request);
            if (!success)
                return NotFound(new { success = false, message = "Schedule summary not found." });

            return Ok(new { success = true, message = "Schedule summary updated successfully." });
        }

        [HttpPost("patient-self-reminder")]
        public async Task<IActionResult> CreatePatientSelfReminder([FromBody] CreatePatientSelfReminderRequestDto request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            var patientId = GetLoggedInPatientId();
            if (patientId == null)
                return Unauthorized(new { success = false, message = "Patient not logged in." });

            var success = await _orderProductScheduleService.CreatePatientSelfReminderAsync(request, patientId.Value);

            if (!success)
                return StatusCode(500, new { success = false, message = "Failed to create reminder." });

            return Ok(new { success = true, message = "Patient self reminder created successfully." });
        }

        [HttpPost("patient-self-reminder/filter")]
        public async Task<IActionResult> GetPatientSelfReminders([FromBody] PatientSelfReminderFilterRequestDto request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            if (request.EndDate < request.StartDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "End date must be on or after start date."
                });
            }

            var patientId = GetLoggedInPatientId();
            if (patientId == null)
                return Unauthorized(new { success = false, message = "Patient not logged in." });

            var reminders = await _orderProductScheduleService
                .GetPatientSelfRemindersAsync(patientId.Value, request.StartDate, request.EndDate);

            return Ok(new
            {
                success = true,
                message = "Success",
                reminders
            });
        }
    }
}
