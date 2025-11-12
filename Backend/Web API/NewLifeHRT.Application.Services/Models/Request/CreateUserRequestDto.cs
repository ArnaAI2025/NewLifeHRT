using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CreateUserRequestDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one role must be provided.")]
        public List<int> RoleIds { get; set; } = new();
        public Guid? PatientId { get; set; }
        public AddressDto? Address {  get; set; }
        public string? DEA { get; set; }
        public string? NPI { get; set; }
        public decimal? CommisionInPercentage { get; set; }
        public bool? MatchAsCommisionRate { get; set; }
        public string? ReplaceCommisionRate { get; set; }
        public bool IsVacationApplicable {  get; set; }
        public List<Guid>? ServiceIds { get; set; }
        public List<LicenseInformationRequestDto>? LicenseInformation { get; set; }
        public int? TimezoneId { get; set; }    
        public string? Color { get; set; }
        public IFormFile? SignatureFile { get; set; }
        public bool MustChangePassword { get; set; } = false;
    }
}
