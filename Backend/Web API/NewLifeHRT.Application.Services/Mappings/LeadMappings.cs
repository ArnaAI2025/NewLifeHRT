using NewLifeHRT.Application.DTOs.Leads;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class LeadMappings
    {
        public static LeadResponseDto ToLeadResponseDto(this Lead lead)
        {
            return new LeadResponseDto
            {
                Id = lead.Id,
                Subject = lead.Subject,
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                PhoneNumber = lead.PhoneNumber,
                Email = lead.Email,
                DateOfBirth = lead.DateOfBirth,
                Gender = lead.Gender,
                HighLevelOwner = lead.HighLevelOwner,
                Description = lead.Description,
                Tags = lead.Tags,
                AddressId = lead.AddressId,
                OwnerId = lead.OwnerId,
                CreatedAt = lead.CreatedAt,
                IsActive = lead.IsActive,
                IsQualified = lead.IsQualified,
                
                Address = lead.Address is not null ? new AddressDto
                {
                    AddressLine1 = lead.Address.AddressLine1,
                    City = lead.Address.City,
                    StateId = lead.Address.StateId,
                    PostalCode = lead.Address.PostalCode,
                    CountryId = lead.Address.CountryId
                } : null,

                OwnerFullName = lead.Owner is not null
                    ? $"{(lead.Owner.FirstName ?? "").Trim()} {(lead.Owner.LastName ?? "").Trim()}".Trim()
                    : null,
                
            };
        }

        public static List<LeadResponseDto> ToLeadResponseDtoList(this IEnumerable<Lead> leads)
        {
            return leads.Select(lead => lead.ToLeadResponseDto()).ToList();
        }
        public static CreatePatientRequestDto ToCreatePatientRequestDto(this Lead lead)
        {           
                return new CreatePatientRequestDto
                {
                    FirstName = lead.FirstName,
                    LastName = lead.LastName,
                    PhoneNumber = lead.PhoneNumber,
                    Email = lead.Email,
                    DateOfBirth = lead.DateOfBirth,
                    Gender = lead.Gender.HasValue ? (int)lead.Gender.Value : null, 
                    CounselorId = lead.OwnerId,
                    AddressId = lead.AddressId,
                    IsFromLead = true,
                    Status = true,
                    Address = new AddressDto()
                    {
                        Id = lead.AddressId,
                        AddressLine1 = lead.Address?.AddressLine1,
                        AddressType = lead.Address?.AddressType,
                        City = lead.Address?.City,
                        PostalCode = lead.Address?.PostalCode,
                        StateId = lead.Address?.StateId,
                        CountryId = lead.Address?.CountryId
                    }
                };           
        }
        public static List<CreatePatientRequestDto> ToCreatePatientRequestDtoList(this IEnumerable<Lead> leads)
        {
            return leads.Select(lead => lead.ToCreatePatientRequestDto()).ToList();
        }
        public static PatientLeadCommunicationDropdownDto ToLeadCommunicationDropdownDto(this Lead lead)
        {
            return new PatientLeadCommunicationDropdownDto
            {
                Id = lead.Id,
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                Email = lead.Email,
                PhoneNumber = lead.PhoneNumber,
                StateId = lead.Address?.StateId,
            };
        }

        public static List<PatientLeadCommunicationDropdownDto> ToLeadCommunicationDropdownDtoList(this IEnumerable<Lead> patients)
        {
            return patients.Select(p => p.ToLeadCommunicationDropdownDto()).ToList();
        }

    }
}
