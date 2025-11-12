using NewLifeHRT.Application.Services.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PatientResponseDto
    {
        public Guid Id { get; set; }
        public int? VisitTypeId { get; set; }
        public bool? SplitCommission { get; set; }
        public string? PatientGoal { get; set; }
        public string PatientNumber { get; set; }
        public Guid? ReferralId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PreviousCounselorFullName { get; set; }
        public string Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? DrivingLicence { get; set; }

        public Guid? AddressId { get; set; }
        public AddressDto? Address { get; set; }

        public int? AssignPhysicianId { get; set; }
        public int CounselorId { get; set; }
        public int? PreviousCounselorId { get; set; }
        public string Allergies { get; set; }
        public bool Status { get; set; }
        public bool? IsAllowMail { get; set; }
        public DateTime? LabRenewableAlertDate { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? VisitTypeName { get; set; }
        public string? Code { get; set; }
        public int[] AgendaIds { get; set; } = Array.Empty<int>();
        public List<PatientCreditCardDto> PatientCreditCards { get; set; } = new();
        public string ProfileImageUrl { get; set; }
        public decimal OutstandingRefundBalance { get; set; }
    }
}
