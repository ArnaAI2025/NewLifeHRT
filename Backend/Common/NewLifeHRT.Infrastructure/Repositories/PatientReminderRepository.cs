using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class PatientReminderRepository : Repository<PatientReminder, ClinicDbContext>, IPatientReminderRepository
    {
        public PatientReminderRepository(ClinicDbContext context) : base(context) { }
    }
}
