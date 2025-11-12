using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class LeadReminderRepository : Repository<LeadReminder, ClinicDbContext>, ILeadReminderRepository
    {
        public LeadReminderRepository(ClinicDbContext context) : base(context) { }
    }
}
