using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Common.Interfaces;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Services
{
    internal class ClinicMigrationService : IClinicMigrationService
    {
        private readonly ClinicDbContext _clinicDbContext;
        private readonly ILogger<ClinicMigrationService> _logger;

        public ClinicMigrationService(ClinicDbContext clinicDbContext, ILogger<ClinicMigrationService> logger)
        {
            _clinicDbContext = clinicDbContext;
            _logger = logger;
        }

        public async Task SetupClinicsDatabaseAsync(MultiTenantInfo multiTenantInfo)
        {
            _logger.LogInformation("Starting database migration for tenant {TenantId}, DB: {DatabaseName}",multiTenantInfo.Identifier, multiTenantInfo.DatabaseName);
            Console.WriteLine($"Applying migrations for: {multiTenantInfo.Identifier}");

            try
            {
                // TODO: Each application startup now reaches this code so there could be a race condition if two apps start at the same time.
                // One possible solution is moving migrations to a console app and running multiple apps with docker compose, so the NewLifeHRT apps can start after migrations.
                await _clinicDbContext.Database.MigrateAsync();
                _logger.LogInformation("Successfully applied migrations for tenant {TenantId}, DB: {DatabaseName}",multiTenantInfo.Identifier, multiTenantInfo.DatabaseName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to migrate development database \"{migrationDbName}\"", multiTenantInfo.DatabaseName);
                throw;
            }

        }
    }
}
