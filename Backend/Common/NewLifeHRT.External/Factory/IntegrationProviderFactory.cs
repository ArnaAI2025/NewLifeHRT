using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.External.Factory.Provider;
using NewLifeHRT.External.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Factory
{
    public class IntegrationProviderFactory : IIntegrationProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public IntegrationProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IIntegrationProvider GetIntegrationProvider(string type)
        {
            return type.ToLower() switch
            {
                "lifefile" => _serviceProvider.GetRequiredService<LifeFileIntegrationProvider>(),
                "wells" => _serviceProvider.GetRequiredService<WellsIntegrationProvider>(),
                "empower" => _serviceProvider.GetRequiredService<EmpowerIntegrationProvider>(),
                _ => throw new ArgumentException($"Integration provider '{type}' is not supported")
            };
        }
    }
}
