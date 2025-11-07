using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Interfaces
{
    public interface ISingletonService
    {
        Task<bool> CheckOverlapAsync(int doctorId, DateOnly date, TimeOnly start, TimeOnly end, IAppointmentRepository appointmentRepository, IHolidayRepository holidayRepository);
        void ReserveSlot(int doctorId, DateOnly date, TimeOnly start, TimeOnly end);
        void ReleaseSlot(int doctorId, DateOnly date, TimeOnly start, TimeOnly end);
    }
}
