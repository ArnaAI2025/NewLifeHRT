using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Models;
using NewLifeHRT.External.Services;
using NewLifeHRT.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.StartupSection
{
    public static class ThirdPartyServiceInitializers
    {
        public static IServiceCollection AddThirdPartyServices(this IServiceCollection services)
        {
            services.AddHttpClient<ISmsService, TwilioSmsService>();
            services.AddScoped<IAudioConverter, AudioConverter>();
            services.AddScoped<IWebhookOrderService, WebhookOrderService>();
            return services;
        }
    }
}
