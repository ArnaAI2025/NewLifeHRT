using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class GetRolesForCreateUserResponseDto : CommonDropDownResponseDto<int>
    {
        public string RoleEnum { get; set; }
    }
}
