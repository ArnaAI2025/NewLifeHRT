using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Interfaces
{
    public interface IClinicMigrationService
    {
        Task SetupClinicsDatabaseAsync(MultiTenantInfo multiTenantInfo);
    }
}
