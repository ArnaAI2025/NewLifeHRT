using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class RoleMappings
    {
        public static GetRolesForCreateUserResponseDto ToGetRolesForCreateUserResponseDto(this ApplicationRole role)
        {
            return new GetRolesForCreateUserResponseDto
            {
                Id = role.Id,
                Value = role.Name,
                RoleEnum = role.RoleEnum.ToString(),
            };
        }
        public static List<GetRolesForCreateUserResponseDto> ToGetRolesForCreateUserResponseDtoList(this IEnumerable<ApplicationRole> roles)
        {
            return roles.Select(p => p.ToGetRolesForCreateUserResponseDto()).ToList();
        }
    }
}
