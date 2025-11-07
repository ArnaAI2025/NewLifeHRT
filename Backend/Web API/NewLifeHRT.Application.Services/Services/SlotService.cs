using NewLifeHRT.Common.Helpers;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
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

            var doctor = await _userRepository.GetWithIncludeAsync(doctorId, new[] { "Timezone" });
            if (doctor == null || doctor.Timezone == null)
                throw new Exception("Doctor or timezone not found");

            var tz = TimeZoneInfo.FindSystemTimeZoneById(doctor.Timezone.StandardName);
            var doctorCurrentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var slotDtos = new List<SlotResponseDto>();

            foreach (var slot in slotList)
            {
                bool isBooked = OverlapHelper.HasAppointmentOverlap(appointmentList, appointmentDate, slot.StartTime, slot.EndTime);

                bool isHoliday = OverlapHelper.HasHolidayOverlap(holidayList, appointmentDate, slot.StartTime, slot.EndTime);
                var appointmentStartDateTime = appointmentDate.ToDateTime(slot.StartTime);
                bool isAlreadyPassed = doctorCurrentDateTime > appointmentStartDateTime;
                slotDtos.Add(new SlotResponseDto
                {
                    SlotId = slot.Id,
                    StartTime = slot.StartTime.ToString("hh:mm tt"),
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
