using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Enums;
using System;

namespace NewLifeHRT.Application.DTOs.Leads
{
    public class LeadResponseDto
    {
        public Guid Id { get; set; }

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
        public AddressDto? Address { get; set; }

        public int OwnerId { get; set; }
        public string? OwnerFullName { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string? CreatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsQualified { get; set; }
    }
}
