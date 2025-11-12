using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Interfaces
{
    public interface IIntegrationProviderFactory
    {
        IIntegrationProvider GetIntegrationProvider(string type);
    }
}
