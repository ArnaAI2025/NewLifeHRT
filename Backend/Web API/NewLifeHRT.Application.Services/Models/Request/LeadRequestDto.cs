using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class LeadRequestDto
    {
        public string Subject { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? HighLevelOwner { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }

        public Guid? AddressId { get; set; }  
        public int OwnerId { get; set; }      
        public AddressDto? AddressDto { get; set; }
    }

}
