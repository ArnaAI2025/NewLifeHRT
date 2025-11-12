using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Settings
{
    public class AzureBlobStorageSettings
    {
        public string ContainerSasUrl { get; set; }
        public string SasToken { get; set; }
    }
}
