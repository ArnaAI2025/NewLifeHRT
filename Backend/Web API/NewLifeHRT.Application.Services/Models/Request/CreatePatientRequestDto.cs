using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CreatePatientRequestDto
    {
        public int? VisitTypeId { get; set; }
        public bool? SplitCommission { get; set; }
        public string? PatientGoal { get; set; }
        public string? PatientNumber { get; set; }
        public Guid? ReferralId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? DrivingLicence { get; set; }
        public Guid? AddressId { get; set; }
        public int? AssignPhysicianId { get; set; }
        public int? CounselorId { get; set; }
        public int? PreviousCounselorId { get; set; }
        public string Allergies { get; set; }
        public bool Status { get; set; }
        public bool? IsAllowMail { get; set; }
        public DateTime? LabRenewableAlertDate { get; set; }
        public AddressDto? Address { get; set; }
        public int[]? AgendaId { get; set; }
        public List<PatientCreditCardDto>? PatientCreditCards { get; set; }
        public IFormFile? ProfileImageFile { get; set; }
        public int? ProfileImageFileCategoryId { get; set; } = 4;
        public bool? IsFromLead { get; set; }
        public decimal OutstandingRefundBalance { get; set; } = 0;

    }

}
