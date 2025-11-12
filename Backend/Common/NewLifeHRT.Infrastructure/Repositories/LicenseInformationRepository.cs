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
    public class LicenseInformationRepository : Repository<LicenseInformation, ClinicDbContext>, ILicenseInformationRepository
    {
        public LicenseInformationRepository(
            ClinicDbContext context
        ) : base(context)
        {

        }

    }
}
