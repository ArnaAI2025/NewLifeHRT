using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.StartupSection
{
    public static class ThirdPartySettingsInitializers
    {
        public static IServiceCollection RegisterThirdPartySettingsInitializers(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TwilioSettings>(configuration.GetSection(AppSettingKeys.TwilioSettings));
            return services;
        }
    }
}
