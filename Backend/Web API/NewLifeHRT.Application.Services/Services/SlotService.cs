using NewLifeHRT.Common.Helpers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _slotRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IUserRepository _userRepository;

        public SlotService(ISlotRepository slotRepository, IAppointmentRepository appointmentRepository, IHolidayRepository holidayRepository, IUserRepository userRepository)
        {
            _slotRepository = slotRepository;
            _appointmentRepository = appointmentRepository;
            _holidayRepository = holidayRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves all available time slots for a specific doctor and date.
        /// Includes logic for checking overlapping appointments, holidays, and already passed slots.
        /// </summary>
        /// <param name="serviceLinkId">Unique identifier for the service link associated with the doctor.</param>
        /// <param name="doctorId">Unique doctor ID.</param>
        /// <param name="appointmentDate">The date for which available slots are fetched.</param>
        /// <returns>A list of slot DTOs with status flags (Booked, Holiday, Passed).</returns>
        public async Task<IEnumerable<SlotResponseDto>> GetAllSlotsAsync(Guid serviceLinkId, int doctorId, DateOnly appointmentDate)
        {
            var slots = await _slotRepository.FindAsync(s => s.UserServiceLinkId == serviceLinkId);
            var slotList = slots.ToList();

            var doctorAppointments = await _appointmentRepository.FindAsync(
                a => a.DoctorId == doctorId && a.AppointmentDate == appointmentDate
            );

            var appointmentList = doctorAppointments.ToList();

            var doctorHolidays = await _holidayRepository.GetHolidaysForDoctorAsync(doctorId, appointmentDate);
            var holidayList = doctorHolidays.ToList();

            // Get doctor’s timezone to accurately calculate if a slot has already passed
            var doctor = await _userRepository.GetWithIncludeAsync(doctorId, new[] { "Timezone" });
            if (doctor == null || doctor.Timezone == null)
            {
                throw new Exception("Doctor or timezone not found");
            }

            var tz = TimeZoneInfo.FindSystemTimeZoneById(doctor.Timezone.StandardName);
            var doctorCurrentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var slotDtos = new List<SlotResponseDto>();

            foreach (var slot in slotList)
            {
                //  Check for appointment overlap using helper method
                bool isBooked = OverlapHelper.HasAppointmentOverlap(appointmentList, appointmentDate, slot.StartTime, slot.EndTime);

                //  Check if slot overlaps with a doctor’s holiday period
                bool isHoliday = OverlapHelper.HasHolidayOverlap(holidayList, appointmentDate, slot.StartTime, slot.EndTime);

                // Calculate if the slot time has already passed based on doctor’s current time
                var appointmentStartDateTime = appointmentDate.ToDateTime(slot.StartTime);
                bool isAlreadyPassed = doctorCurrentDateTime > appointmentStartDateTime;
                slotDtos.Add(new SlotResponseDto
                {
                    SlotId = slot.Id,
                    StartTime = slot.StartTime.ToString("hh:mm tt"),// Hardcoded 12-hour format for user readability
                    EndTime = slot.EndTime.ToString("hh:mm tt"),
                    IsBooked = isBooked,
                    IsHoliday = isHoliday,
                    IsAlreadyPassed = isAlreadyPassed
                });
            }
            return slotDtos;
        }
    }
}
