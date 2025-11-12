using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class PatientCreditCardRepository : Repository<PatientCreditCard, ClinicDbContext>, IPatientCreditCardRepository
    {
        public PatientCreditCardRepository(
            ClinicDbContext context
        ) : base(context)
        {

        }
    
    }
}
