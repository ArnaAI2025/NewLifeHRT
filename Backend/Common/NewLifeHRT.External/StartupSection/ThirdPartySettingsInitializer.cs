using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.External.Settings;
using NewLifeHRT.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.StartupSection
{
    public static class ThirdPartySettingsInitializer
    {
        public static IServiceCollection RegisterThirdPartySettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TwilioSettings>(configuration.GetSection(AppSettingKeys.TwilioSettings));
            return services;
        }
    }
}
