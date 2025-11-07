using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class GetActiveUsersRequest
    {
        public int RoleId { get; set; }
        public string? SearchTerm { get; set; } = string.Empty;
    }

}
