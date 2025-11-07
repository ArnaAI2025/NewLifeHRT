using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class DbContextInitializer
    {
        public static IServiceCollection RegisterHospitalDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HospitalDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(AppSettingKeys.ConnectionStringKeys.HospitalDatabase));
            });
            return services;
        }
        public static IServiceCollection RegisterClinicDbContext(this IServiceCollection services)
        {
            services.AddDbContext<ClinicDbContext>();
            return services;
        }
    }
}
