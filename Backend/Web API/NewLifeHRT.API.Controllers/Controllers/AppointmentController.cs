using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class AppointmentController : BaseApiController<AppointmentController>
    {
        private readonly ISlotService _slotService;
        private readonly IAppointmentModeService _appointmentModeService;
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(ISlotService slotService, IAppointmentModeService appointmentModeService, IAppointmentService appointmentService)
        {
            _slotService = slotService;
            _appointmentModeService = appointmentModeService;
            _appointmentService = appointmentService;
        }

        [HttpGet("get-all-slots")]
        public async Task<IActionResult> GetAllSlots([FromQuery] Guid serviceLinkId, [FromQuery] int doctorId, [FromQuery] DateOnly appointmentDate)
        {
            if (serviceLinkId == Guid.Empty || doctorId <= 0 || appointmentDate == default)
                return BadRequest("Invalid parameters.");

            var slots = await _slotService.GetAllSlotsAsync(serviceLinkId, doctorId, appointmentDate);

            return Ok(slots);
        }

        [HttpGet("get-all-appointmentmodes")]
        public async Task<IActionResult> GetAllAppointmentModes()
        {
            var modes = await _appointmentModeService.GetAllAppointmentModesAsync();
            return Ok(modes);
        }

        [HttpPost("get-all-appointments")]
        public async Task<IActionResult> GetAllAppointments([FromBody] GetAllAppointmentsRequestDto requestDTO)
        {
            if (requestDTO.StartDate == default || requestDTO.EndDate == default)
                return BadRequest("StartDate and EndDate are required.");

            var result = await _appointmentService.GetAppointmentsAsync(requestDTO.StartDate, requestDTO.EndDate, requestDTO.DoctorIds);

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto request)
        {
            if (request == null || request.DoctorId <= 0 || request.PatientId == Guid.Empty)
                return BadRequest("Invalid request.");

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _appointmentService.CreateAppointmentAsync(request, userId.Value);
            if (!result.Success)
                return Conflict(result.Message);

            return Ok(new { Message = result.Message, AppointmentId = result.AppointmentId });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] CreateAppointmentRequestDto request)
        {
            if (request == null || request.DoctorId <= 0 || request.PatientId == Guid.Empty)
                return BadRequest("Invalid request.");

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            var result = await _appointmentService.UpdateAppointmentAsync(request, id ,userId.Value);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    "NotFound" => NotFound(new { Message = result.Message }),
                    "Overlap" => Conflict(new { Message = result.Message }),
                    _ => BadRequest(new { Message = result.Message })
                };
            }

            return Ok(new { Message = result.Message, AppointmentId = result.AppointmentId });
        }

        [HttpDelete("delete/{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(Guid appointmentId)
        {
            if (appointmentId == Guid.Empty)
                return BadRequest("Invalid appointment Id.");

            var result = await _appointmentService.DeleteAppointmentAsync(appointmentId);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result);
        }

        [HttpGet("get/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById(Guid appointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if(appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        [HttpGet("get-by-patientId/{patientId}")]
        public async Task<IActionResult> GetAppointmentByPatientId(Guid patientId)
        {
            var appointment = await _appointmentService.GetAppointmentByPatientIdAsync(patientId);
            return Ok(appointment);
        }
    }
 }
