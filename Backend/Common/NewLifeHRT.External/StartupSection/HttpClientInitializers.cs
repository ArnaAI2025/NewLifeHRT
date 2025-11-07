using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.StartupSection
{
    public static class HttpClientInitializers
    {
        public static IServiceCollection AddHttpClientInitializers(this IServiceCollection services)
        {
            return services;
        }
    }
}
