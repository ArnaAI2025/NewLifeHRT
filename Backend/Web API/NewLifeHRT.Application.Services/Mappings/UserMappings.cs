using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class UserMappings
    {
        public static UserResponseDto ToUserResponseDto(this ApplicationUser user)
        {
            var roleIds = user.UserRoles?.Select(ur => ur.RoleId).Distinct().ToArray() ?? Array.Empty<int>();

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleIds = roleIds,
                DEA = user.DEA,
                NPI = user.NPI,
                CommisionInPercentage = user.CommisionInPercentage,
                MatchAsCommisionRate = user.MatchAsCommisionRate,
                ReplaceCommisionRate = user.ReplaceCommisionRate,
                IsVacationApplicable = user.Vacation,
                TimezoneId = user.TimezoneId,
                Color = user.ColorCode,
                CreatedAt = user.CreatedAt,                
                IsDeleted = user.IsDeleted,
                SignatureUrls = user.UserSignatures?.Where(s => s.IsActive)
                        .Select(s => new UserSignatureResponseDto
                        {
                            Id = s.Id,
                            Url = s.SignaturePath,
                        })
                        .ToList() ?? new List<UserSignatureResponseDto>(),
                Address = user.Address != null ? new AddressDto
                {
                    AddressLine1 = user.Address.AddressLine1,
                    City = user.Address.City,
                    StateId = user.Address.StateId,
                    PostalCode = user.Address.PostalCode,
                    CountryId = user.Address.CountryId,
                    Country = user.Address?.Country?.Name
                } : null,
                LicenseInformations = user.LicenseInformations?
                    .Select(sm => new LicenseInformationResponseDto
                    {
                        Id = sm.Id,
                        StateId = sm.StateId,
                        StateName = sm.State?.Name ?? String.Empty,
                        Number = sm.Number,
                    })
                    .ToArray() ?? Array.Empty<LicenseInformationResponseDto>(),
                ServiceIds = user.UserServices?.Select(us => us.ServiceId).ToArray()
            };
        }

        public static List<UserResponseDto> ToUserResponseDtoList(this IEnumerable<ApplicationUser> users)
        {
            return users.Select(user => user.ToUserResponseDto()).ToList();
        }

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
