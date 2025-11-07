using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public int ExpiryTime { get; set; }
    }
}
