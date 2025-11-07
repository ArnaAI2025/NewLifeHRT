using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.DTOs.Leads;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAddressRepository _addressRepository;

        public LeadService(
            ILeadRepository leadRepository,
            IAddressRepository addressRepository,
            ClinicDbContext context)
        {
            _leadRepository = leadRepository;
            _addressRepository = addressRepository;
        }

        public async Task<CommonOperationResponseDto<Guid?>> CreateAsync(LeadRequestDto createLeadRequestDto, int userId)
        {
            var lead = new Lead
            {
                Subject = createLeadRequestDto.Subject,
                FirstName = createLeadRequestDto.FirstName,
                LastName = createLeadRequestDto.LastName,
                PhoneNumber = createLeadRequestDto.PhoneNumber,
                Email = createLeadRequestDto.Email,
                DateOfBirth = createLeadRequestDto.DateOfBirth,
                Gender = createLeadRequestDto.Gender,
                HighLevelOwner = createLeadRequestDto.HighLevelOwner,
                Description = createLeadRequestDto.Description,
                Tags = createLeadRequestDto.Tags,
                OwnerId = createLeadRequestDto.OwnerId,
                CreatedBy = userId.ToString(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsQualified = null
            };

            if (createLeadRequestDto.AddressDto != null)
            {
                var address = new Address
                {
                    AddressLine1 = createLeadRequestDto.AddressDto.AddressLine1,
                    City = createLeadRequestDto.AddressDto.City,
                    StateId = createLeadRequestDto.AddressDto.StateId,
                    PostalCode = createLeadRequestDto.AddressDto.PostalCode,
                    CountryId = createLeadRequestDto.AddressDto.CountryId,
                    CreatedBy = userId.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                address = await _addressRepository.AddAsync(address);
                lead.AddressId = address.Id;
            }

            try
            {
                await _leadRepository.AddAsync(lead);

            }
            catch (Exception ex)
            {

                throw;
            }
            return new CommonOperationResponseDto<Guid?>
            {
                Id = lead.Id,
                Message = "Lead created suceessfully"
            };
        }

        public async Task<IList<LeadResponseDto>> GetAllAsync(int? ownerId = null)
        {
            var predicates = new List<Expression<Func<Lead, bool>>>();

            if (ownerId.HasValue)
                predicates.Add(l => l.OwnerId == ownerId.Value);
            else
                predicates.Add(l => true);

            var leads = await _leadRepository.FindWithIncludeAsync(predicates, new[] { "Address", "Owner" }, noTracking: true);

            return LeadMappings.ToLeadResponseDtoList(leads);
        }


        public async Task<LeadResponseDto?> GetByIdAsync(Guid id)
        {
            var lead = await _leadRepository.GetWithIncludeAsync(id, new[] { "Address", "Owner" });
            if (lead == null) return null;

            return LeadMappings.ToLeadResponseDto(lead);
        }
        public async Task<List<PatientLeadCommunicationDropdownDto>> GetAllOnCounselorIdAsync(int counselorId)
        {
            var predicates = new List<Expression<Func<Lead, bool>>>
            {
                p => p.OwnerId == counselorId
            };
            var includes = new string[] { nameof(Lead.Address) };
            var activePatients = await _leadRepository.FindWithIncludeAsync(predicates, includes, noTracking: true);
            return activePatients.ToLeadCommunicationDropdownDtoList();
        }

        public async Task<CommonOperationResponseDto<Guid?>> UpdateAsync(Guid id, LeadRequestDto updateLeadRequestDto, int userId)
        {
            var lead = await _leadRepository.GetWithIncludeAsync(id, new[] { "Address" });
            if (lead == null || !lead.IsActive)
                throw new Exception("Lead not found or inactive.");

            lead.Subject = updateLeadRequestDto.Subject;
            lead.FirstName = updateLeadRequestDto.FirstName;
            lead.LastName = updateLeadRequestDto.LastName;
            lead.PhoneNumber = updateLeadRequestDto.PhoneNumber;
            lead.Email = updateLeadRequestDto.Email;
            lead.DateOfBirth = updateLeadRequestDto.DateOfBirth;
            lead.Gender = updateLeadRequestDto.Gender;
            lead.HighLevelOwner = updateLeadRequestDto.HighLevelOwner;
            lead.Description = updateLeadRequestDto.Description;
            lead.Tags = updateLeadRequestDto.Tags;
            lead.OwnerId = updateLeadRequestDto.OwnerId;
            lead.UpdatedAt = DateTime.UtcNow;
            lead.UpdatedBy = userId.ToString();

            if (updateLeadRequestDto.AddressDto != null)
            {
                if (lead.Address == null)
                {
                    var newAddress = new Address
                    {
                        CreatedBy = userId.ToString(),
                        CreatedAt = DateTime.UtcNow
                    };
                    lead.Address = await _addressRepository.AddAsync(newAddress);
                }

                lead.Address.AddressLine1 = updateLeadRequestDto.AddressDto.AddressLine1;
                lead.Address.City = updateLeadRequestDto.AddressDto.City;
                lead.Address.StateId = updateLeadRequestDto.AddressDto.StateId;
                lead.Address.PostalCode = updateLeadRequestDto.AddressDto.PostalCode;
                lead.Address.CountryId = updateLeadRequestDto.AddressDto.CountryId;
                lead.Address.UpdatedAt = DateTime.UtcNow;
                lead.Address.UpdatedBy = userId.ToString();

                await _addressRepository.UpdateAsync(lead.Address);
            }

            await _leadRepository.UpdateAsync(lead);
            return new CommonOperationResponseDto<Guid?>
            {
                Id = lead.Id,
                Message = "Lead created suceessfully"
            };
        }
        
        public async Task<CommonOperationResponseDto<int>> BulkToggleActiveStatusAsync(IEnumerable<Guid> leadIds, int userId, bool isActive)
        {
            
                if (leadIds == null || !leadIds.Any())
                {
                    return new CommonOperationResponseDto<int>
                    {
                        Id = 0,
                        Message = "No valid Lead IDs provided."
                    };
                }
                var idList = leadIds.ToList();

            var leadsToUpdate = (await _leadRepository.FindAsync( l => idList.Contains(l.Id), noTracking: false)).ToList();


            if (leadsToUpdate == null || !leadsToUpdate.Any())
                {
                    return new CommonOperationResponseDto<int>
                    {
                        Id = 0,
                        Message = "No leads found for the provided IDs."
                    };
                }

                foreach (var lead in leadsToUpdate)
                {
                    lead.IsActive = isActive;
                    lead.UpdatedBy = userId.ToString();
                    lead.UpdatedAt = DateTime.UtcNow;
                }

                await _leadRepository.BulkUpdateAsync(leadsToUpdate);

                return new CommonOperationResponseDto<int>
                {
                    Id = leadsToUpdate.Count(),
                    Message = isActive ? "Leads activated successfully." : "Leads deactivated successfully."
                };           
        }
        public async Task<CommonOperationResponseDto<int>> BulkAssignLeadsAsync(IEnumerable<Guid> leadIds, int assigneeId, int userId)
        {
            if (leadIds == null || !leadIds.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No valid Lead IDs provided."
                };
            }           
 
            var leadsToAssign = (await _leadRepository.FindAsync(l => leadIds.Contains(l.Id), noTracking: false)).ToList();

            if (leadsToAssign == null || !leadsToAssign.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No leads found for the provided IDs."
                };
            }

            foreach (var lead in leadsToAssign)
            {
                lead.OwnerId = assigneeId;
                lead.UpdatedBy = userId.ToString();
                lead.UpdatedAt = DateTime.UtcNow;
            }

            await _leadRepository.BulkUpdateAsync(leadsToAssign);

            return new CommonOperationResponseDto<int>
            {
                Id = leadsToAssign.Count(),
                Message = $"{leadsToAssign.Count()} lead(s) successfully assigned to counselor"
            };
        }
        public async Task<List<CreatePatientRequestDto>> ConvertToPatientRequestAsync(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return new List<CreatePatientRequestDto>();

            var predicates = new List<Expression<Func<Lead, bool>>>
                                {
                                    l => ids.Contains(l.Id)
                                };

            var leads = await _leadRepository.FindWithIncludeAsync( predicates, new[] { "Address" }, noTracking: true );
            return LeadMappings.ToCreatePatientRequestDtoList(leads);
        }

        public async Task<CommonOperationResponseDto<int>> BulkToggleIsQualifiedAsync(IEnumerable<Guid> leadIds, bool isQualified, int userId)
        {
            if (leadIds == null || !leadIds.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No valid Lead IDs provided."
                };
            }

            var leadsToAssign = (await _leadRepository.FindAsync(l => leadIds.Contains(l.Id), noTracking: false)).ToList();

            if (leadsToAssign == null || !leadsToAssign.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No leads found for the provided IDs."
                };
            }

            foreach (var lead in leadsToAssign)
            {
                lead.IsQualified = isQualified;
                lead.UpdatedBy = userId.ToString();
                lead.UpdatedAt = DateTime.UtcNow;
            }

            await _leadRepository.BulkUpdateAsync(leadsToAssign);

            return new CommonOperationResponseDto<int>
            {
                Id = leadsToAssign.Count(),
                Message = $"{leadsToAssign.Count()} lead(s) {(isQualified ? "qualified" : "disqualified")} successfully"
            };

        }

        public async Task BulkDeleteLeadsAsync(List<Guid> ids, int userId)
        {
            var leads = (await _leadRepository.FindAsync(p => ids.Contains(p.Id))).ToList();

            if (leads == null || !leads.Any())
                throw new Exception("No matching leads found for the IDs.");
            try
            {
                await _leadRepository.BulkDeleteAsync(leads);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while deleting the leads");
            }

        }
        public async Task<Lead?> GetLeadByMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return null;

            // Normalize phone number (basic example — you may want to handle country code more robustly)
            var normalized = mobileNumber.Trim().Replace(" ", "").Replace("-", "");

            var lead = await _leadRepository.GetSingleAsync(
                p => p.PhoneNumber == normalized,
                noTracking: true
            );

            return lead;
        }

    }
}
