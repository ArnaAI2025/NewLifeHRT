using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Common.Interfaces;
using NewLifeHRT.Common.Interfaces.Hospital;
using NewLifeHRT.Common.Services;
using NewLifeHRT.Common.Services.Hospital;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Services;
using NewLifeHRT.Jobs.Scheduler.Interface;
using NewLifeHRT.Jobs.Scheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Startup
{
    public static class SchedularServiceInitializer
    {
        public static IServiceCollection SchedularServices(this IServiceCollection services)
        {
            services.AddHttpClient<ISmsService, TwilioSmsService>();
            services.AddScoped<IOrderProcessingService, OrderProcessingService>();
            services.AddScoped<ISmsSenderService, SmsSenderService>();
            services.AddScoped<IWeeklyCommissionService, WeeklyCommissionService>();
            services.AddScoped<IOrderRefillDateProcessingService, OrderRefillDateProcessingService>();
            services.AddScoped<IAIService, AIService>();
            return services;
        }
    }
}
