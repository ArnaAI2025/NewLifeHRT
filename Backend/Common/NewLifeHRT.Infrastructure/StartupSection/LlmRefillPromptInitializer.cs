using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Domain.Interfaces;
using NewLifeHRT.Infrastructure.Config;
using NewLifeHRT.Infrastructure.Llm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class LlmRefillPromptInitializer
    {
        public static IServiceCollection RegisterRefillDateGenerator(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AISettings>(configuration.GetSection("AISettings"));
            services.AddSingleton<PromptStore>();
            services.AddHttpClient<IRefillDateCalculator, LlmRefillDateCalculator>();
            return services;
        }
    }
}
