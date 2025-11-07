using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PatientMappings
    {
        public static PatientResponseDto ToPatientResponseDto(this Patient patient)
        {
            return new PatientResponseDto
            {
                Id = patient.Id,
                VisitTypeId = patient.VisitTypeId,
                SplitCommission = patient.SplitCommission,
                PatientGoal = patient.PatientGoal,
                PatientNumber = patient.PatientNumber,
                ReferralId = patient.ReferralId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                FullName = string.Join(" ", (patient.FirstName ?? string.Empty).Trim(), (patient.LastName ?? string.Empty).Trim()).Trim(),
                PreviousCounselorFullName = string.Join(" ", (patient?.PreviousCounselor?.FirstName ?? string.Empty).Trim(), (patient?.PreviousCounselor?.LastName ?? string.Empty).Trim()).Trim(),
                Gender = patient.Gender.ToString(),
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                DateOfBirth = patient.DateOfBirth,
                DrivingLicence = patient.DrivingLicence,
                AddressId = patient.AddressId,
                AssignPhysicianId = patient.AssignPhysicianId,
                CounselorId = patient.CounselorId,
                PreviousCounselorId = patient.PreviousCounselorId,
                Allergies = patient.Allergies,
                Status = patient.Status,
                IsAllowMail = patient.IsAllowMail,
                LabRenewableAlertDate = patient.LabRenewableAlertDate,
                IsActive = patient.IsActive,
                OutstandingRefundBalance = patient.OutstandingRefundBalance,
                CreatedDate = patient.CreatedAt,
                CreatedBy = patient.CreatedBy,
                UpdatedAt = patient.UpdatedAt,
                UpdatedBy = patient.UpdatedBy,

                Address = patient.Address is not null ? new AddressDto
                {
                    AddressLine1 = patient.Address.AddressLine1,
                    City = patient.Address.City,
                    StateId = patient.Address.StateId,
                    PostalCode = patient.Address.PostalCode,
                    CountryId = patient.Address.CountryId
                } : null,

                VisitTypeName = patient.VisitType?.VisitTypeName,
                Code = patient.VisitType?.Code,

                AgendaIds = patient.PatientAgendas?.Select(pa => pa.AgendaId).ToArray() ?? Array.Empty<int>(),

                PatientCreditCards = patient.PatientCreditCards != null
            ? patient.PatientCreditCards.Select(cc => new PatientCreditCardDto
            {
                Id = cc.Id,
                CardNumber = cc.CardNumber,
                CardType = (int)cc.CardType,
                Month = (int)cc.Month,
                Year = cc.Year,
                IsActive = cc.IsActive
            }).ToList()
            : new List<PatientCreditCardDto>()            
        };
        }


        public static List<PatientResponseDto> ToPatientResponseDtoList(this IEnumerable<Patient> patients)
        {
            return patients.Select(p => p.ToPatientResponseDto()).ToList();
        }
        public static PatientLeadCommunicationDropdownDto ToPatientCommunicationDropdownDto(this Patient patient)
        {
            return new PatientLeadCommunicationDropdownDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                StateId = patient.Address?.StateId 
            };
        }

        public static List<PatientLeadCommunicationDropdownDto> ToPatientCommunicationDropdownDtoList(this IEnumerable<Patient> patients)
        {
            return patients.Select(p => p.ToPatientCommunicationDropdownDto()).ToList();
        }
    }
}
