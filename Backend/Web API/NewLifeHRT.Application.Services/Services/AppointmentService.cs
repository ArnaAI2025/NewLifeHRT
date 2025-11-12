using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Common.Helpers;
using NewLifeHRT.Common.Interfaces;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly ISingletonService _singletonService;
        private readonly ISlotRepository _slotRepository;
        public AppointmentService(IAppointmentRepository appointmentRepository, IHolidayRepository holidayRepository, ISingletonService singletonService, ISlotRepository slotRepository)
        {
            _appointmentRepository = appointmentRepository;
            _holidayRepository = holidayRepository;
            _singletonService = singletonService;
            _slotRepository = slotRepository;
        }

        /// <summary>
        /// Creates a new appointment after validating slot availability and overlaps.
        /// </summary>
        /// <remarks>
        /// Uses both DB and in-memory singleton overlap checks to avoid double-booking.
        /// </remarks>
        public async Task<AppointmentResultResponseDto> CreateAppointmentAsync(CreateAppointmentRequestDto request, int userId)
        {
            var slot = await _slotRepository.GetByIdAsync(request.SlotId);
            if (slot == null)
            {
                return new AppointmentResultResponseDto
                {
                    Success = false,
                    Message = "Slot not found.",
                    ErrorType = "NotFound"
                };
            }

            //  Check DB-level overlaps to ensure no conflicting appointments
            var existingAppointments = await _appointmentRepository.FindWithIncludeAsync(
                new List<Expression<Func<Appointment, bool>>>
                {
                a => a.DoctorId == request.DoctorId && a.AppointmentDate == request.AppointmentDate
                },
                new[] { "Slot" }
            );

            bool hasDbOverlap = OverlapHelper.HasAppointmentOverlap(
                existingAppointments,
                request.AppointmentDate,
                slot.StartTime,
                slot.EndTime
            );

            if (hasDbOverlap)
            {
                return new AppointmentResultResponseDto
                {
                    Success = false,
                    Message = "The selected slot overlaps with an existing appointment."
                };
            }

            // In-memory overlap validation — prevents race conditions in concurrent slot booking
            bool hasReservedOverlap = await _singletonService.CheckOverlapAsync(
                request.DoctorId,
                request.AppointmentDate,
                slot.StartTime,
                slot.EndTime,
                _appointmentRepository,
                _holidayRepository
            );
            if (hasReservedOverlap)
            {
                return new AppointmentResultResponseDto
                {
                    Success = false,
                    Message = "The selected slot is currently unavailable (reserved or holiday)."
                };
            }

            Appointment appointment;
            var slotReserved = false;
            try
            {
                // Reserve slot temporarily to prevent parallel bookings
                _singletonService.ReserveSlot(request.DoctorId, request.AppointmentDate, slot.StartTime, slot.EndTime);
                slotReserved = true;
                appointment = new Appointment(request.SlotId, request.AppointmentDate, request.PatientId, request.DoctorId, request.ModeId, (int)AppointmentStatusEnum.Confirmed,
                    request.Description, userId.ToString(), DateTime.UtcNow);

                await _appointmentRepository.AddAsync(appointment);
            }
            finally
            {
                if (slotReserved)
                {
                    // Release slot after saving to DB to restore availability control
                    _singletonService.ReleaseSlot(request.DoctorId, request.AppointmentDate, slot.StartTime, slot.EndTime);
                }
            }

            return new AppointmentResultResponseDto
            {
                Success = true,
                Message = "Appointment created successfully.",
                AppointmentId = appointment.Id
            };
        }

        public async Task<AppointmentResultResponseDto> DeleteAppointmentAsync(Guid appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                return new AppointmentResultResponseDto
                {
                    Success = false,
                    Message = "Appointment not found."
                };
            }

            await _appointmentRepository.DeleteAsync(appointment);

            return new AppointmentResultResponseDto
            {
                Success = true,
                Message = "Appointment deleted successfully.",
                AppointmentId = appointment.Id
            };
        }

        public async Task<List<AppointmentResponseDto>> GetAppointmentsAsync(DateOnly startDate, DateOnly endDate, List<int>? doctorIds)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDateRangeAsync(startDate, endDate, doctorIds);

            return appointments.ToAppointmentResponseDtoList();
        }

        /// <summary>
        /// Updates an existing appointment. Performs overlap validation if slot, doctor, or date is changed.
        /// </summary>
        /// <remarks>
        /// Long method: could be separated into smaller private helper methods like:
        ///     - ValidateAppointmentExistence
        ///     - ValidateOverlapBeforeUpdate
        ///     - ApplyAppointmentChanges
        /// </remarks>
        public async Task<AppointmentResultResponseDto> UpdateAppointmentAsync(CreateAppointmentRequestDto request, Guid id, int userId)
        {
            var slot = await _slotRepository.GetByIdAsync(request.SlotId);
            if (slot == null)
            {
                return new AppointmentResultResponseDto
                {
                    Success = false,
                    Message = "Slot not found.",
                    ErrorType = "NotFound"
                };
            }
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return new AppointmentResultResponseDto
                {
                    Success = false,
                    Message = "Appointment not found.",
                    ErrorType = "NotFound"
                };
            }
            // Only check overlaps when critical scheduling fields are changed

            var requiresAvailabilityCheck = appointment.DoctorId != request.DoctorId || appointment.AppointmentDate != request.AppointmentDate || appointment.SlotId != request.SlotId;

            bool slotReserved = false;

            if (requiresAvailabilityCheck)
            {
                var existingAppointments = await _appointmentRepository.FindWithIncludeAsync(
                    new List<Expression<Func<Appointment, bool>>>
                {
                    a => a.DoctorId == request.DoctorId
                     && a.AppointmentDate == request.AppointmentDate
                     && a.Id != id
                },
                new[] { "Slot" }
            );

                bool hasDbOverlap = OverlapHelper.HasAppointmentOverlap(
                    existingAppointments,
                    request.AppointmentDate,
                    slot.StartTime,
                    slot.EndTime
                );

                if (hasDbOverlap)
                {
                    return new AppointmentResultResponseDto
                    {
                        Success = false,
                        Message = "The selected slot overlaps with an existing appointment.",
                        ErrorType = "Overlap"
                    };
                }

                bool hasReservedOverlap = await _singletonService.CheckOverlapAsync(
                    request.DoctorId,
                    request.AppointmentDate,
                    slot.StartTime,
                    slot.EndTime,
                    _appointmentRepository,
                    _holidayRepository
                );

                if (hasReservedOverlap)
                {
                    return new AppointmentResultResponseDto
                    {
                        Success = false,
                        Message = "The selected slot is currently unavailable (reserved or holiday).",
                        ErrorType = "Overlap"
                    };
                }

                _singletonService.ReserveSlot(request.DoctorId, request.AppointmentDate, slot.StartTime, slot.EndTime);
                slotReserved = true;
            }

            // Apply field updates

            appointment.SlotId = request.SlotId;
            appointment.AppointmentDate = request.AppointmentDate;
            appointment.PatientId = request.PatientId;
            appointment.DoctorId = request.DoctorId;
            appointment.ModeId = request.ModeId;
            appointment.Description = request.Description;
            appointment.UpdatedBy = userId.ToString();
            appointment.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _appointmentRepository.UpdateAsync(appointment);
            }
            finally
            {
                if (slotReserved)
                {
                    // Release slot lock post-update
                    _singletonService.ReleaseSlot(request.DoctorId, request.AppointmentDate, slot.StartTime, slot.EndTime);
                }
            }

            return new AppointmentResultResponseDto
            {
                Success = true,
                Message = "Appointment updated successfully.",
                AppointmentId = appointment.Id
            };
        }

        public async Task<AppointmentGetResponseDto?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            var includes = new[] { "Slot.UserServiceLink" };
            var response = await _appointmentRepository.GetWithIncludeAsync(appointmentId, includes);
            return response?.ToAppointmentGetResponseDto();
        }

        public async Task<List<GetAppointmentsByPatientIdResponseDto>> GetAppointmentByPatientIdAsync(Guid patientId)
        {
            var includes = new[]
            {
                "Slot.UserServiceLink",
                "Slot.UserServiceLink.Service",
                "User.Timezone",
                "Patient.Counselor",
                "Status"
            };

            var filters = new List<Expression<Func<Appointment, bool>>>
            {
                a => a.PatientId == patientId
            };

            var result = await _appointmentRepository.FindWithIncludeAsync(filters, includes);
            return result.ToAppointmentGetByPatientIdResponseDtoList();
        }
    }
}
