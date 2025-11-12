using NewLifeHRT.Domain.Entities.Hospital;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Extensions
{
    public static class TenantExtensions
    {
        public static List<MultiTenantInfo> ToMultiTenantInfo(this List<Clinic> query)
        {
            return query.Select(client => new MultiTenantInfo
            {
                Id = client.TenantId.ToString(),
                Identifier = client.Domain.ToLower(),
                ClientId = client.Id,
                Name = client.ClinicName,
                DatabaseName = client.Database,
                IsActive = client.IsActive,
                HostUrl = client.HostUrl,
                JwtBearerAudience = client.JwtBearerAudience,
                IdentityOptions = JsonSerializer.Deserialize<ClinicIdentityOptions>(client.IdentityOptions),
            }).ToList();
        }
    }
}
