using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class DropDownUserMappings
    {
        public static DropDownIntResponseDto ToDropDownUserResponseDto(this ApplicationUser user)
        {
            return new DropDownIntResponseDto
            {
                Id = user.Id,
                Value = $"{user?.FirstName?.Trim()} {user?.LastName?.Trim()}".Trim(),
            };
        }
        public static List<DropDownIntResponseDto> ToDropDownUserResponseDtoList(this IEnumerable<ApplicationUser> users)
        {
            return users.Select(user => user.ToDropDownUserResponseDto()).ToList();
        }
    }
}
