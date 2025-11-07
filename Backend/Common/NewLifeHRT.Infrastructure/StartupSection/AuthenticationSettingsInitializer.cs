using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class AuthenticationSettingsInitializer
    {
        public static void AddAuthenticationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthenticationSettings>(configuration.GetSection(AppSettingKeys.Authentication));
        }
    }
}
