using NewLifeHRT.Application.Services.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int[] RoleIds { get; set; } = Array.Empty<int>();
        public string? DEA { get; set; }
        public string? NPI { get; set; }
        public decimal? CommisionInPercentage { get; set; }
        public bool? MatchAsCommisionRate { get; set; }
        public string? ReplaceCommisionRate { get; set; }
        public bool IsVacationApplicable { get; set; }
        public int? TimezoneId { get; set; }
        public string? Color { get; set; }
        public AddressDto? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid[] ServiceIds { get; set; }
        public bool? IsDeleted { get; set; }
        public string SignatureUrl { get; set; }

        public LicenseInformationResponseDto[]? LicenseInformation { get; set; }
    }
}
