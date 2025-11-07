using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Design;
using NewLifeHRT.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using Microsoft.Extensions.Logging;

namespace MultiTenantTest2
{
    public class ClinicContextDesignTimeFactory : IDesignTimeDbContextFactory<ClinicDbContext>
    {
        public ClinicDbContext CreateDbContext(string[] args)
        {
            var services = new ServiceCollection();
            services.AddMultiTenant<MultiTenantInfo>()
                .WithStore<MultiTenantInfoStore>(ServiceLifetime.Singleton);

            var serviceProvider = services.BuildServiceProvider();

            var setter = serviceProvider.GetRequiredService<IMultiTenantContextSetter>();

            var tenantInfo = new MultiTenantInfo
            {
                ConnectionString = "data source=localhost;initial catalog=CLINICDBDEMO;integrated security=True;multipleactiveresultsets=False;Encrypt=False;"
            };

            var multiTenantContext = new MultiTenantContext<MultiTenantInfo>
            {
                TenantInfo = tenantInfo
            };
            setter.MultiTenantContext = multiTenantContext;
            var accessor = serviceProvider.GetRequiredService<IMultiTenantContextAccessor<MultiTenantInfo>>();

            var options = new DbContextOptionsBuilder<ClinicDbContext>()
                .UseSqlServer(tenantInfo.ConnectionString)
                .Options;

            return new ClinicDbContext(
                options,
                accessor,
                LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ClinicDbContext>());
        }
    }
}
