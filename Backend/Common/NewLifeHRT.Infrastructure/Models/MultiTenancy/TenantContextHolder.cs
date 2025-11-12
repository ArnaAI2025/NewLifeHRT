using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Models.MultiTenancy
{
    public static class TenantContextHolder
    {
        public static string? CurrentTenant { get; set; }
    }
}
