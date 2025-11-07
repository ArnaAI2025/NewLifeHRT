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
    public class LeadReminderRepository : Repository<LeadReminder, ClinicDbContext>, ILeadReminderRepository
    {
        public LeadReminderRepository(ClinicDbContext context) : base(context) { }
    }
}
