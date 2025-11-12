using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Common.Services.Llm;
using NewLifeHRT.Common.Settings;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.StartupSection
{
    public static class LlmRefillPromptInitializer
    {
        public static IServiceCollection RegisterRefillDateGenerator(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AISettings>(configuration.GetSection(AppSettingKeys.AISettings));
            services.AddSingleton<PromptStore>();
            services.AddHttpClient<IRefillDateCalculator, LlmRefillDateCalculator>();
            return services;
        }
    }
}
