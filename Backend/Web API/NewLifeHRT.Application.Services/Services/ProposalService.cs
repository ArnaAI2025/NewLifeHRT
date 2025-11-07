using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Api.Requests;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.DTOs;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly IProposalDetailService _proposalDetailService;
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;
        private readonly IPatientCreditCardRepository _patientCreditCardRepository;
        private readonly IPharmacyShippingMethodService _pharmacyShippingMethodService;
        public ProposalService(IProposalRepository proposalRepository, IProposalDetailService proposalDetailService, IShippingAddressService shippingAddressService, IAddressService addressService, IOrderService orderService, IPharmacyShippingMethodService pharmacyShippingMethodService, IPatientCreditCardRepository patientCreditCardRepository)
        {
            _proposalRepository = proposalRepository;
            _proposalDetailService = proposalDetailService;
            _shippingAddressService = shippingAddressService;
            _addressService = addressService;
            _orderService = orderService;
            _pharmacyShippingMethodService = pharmacyShippingMethodService;
            _patientCreditCardRepository = patientCreditCardRepository;
        }
        public async Task<List<ProposalBulkResponseDto>> GetAllAsync(List<int>? ids, Guid? patientId)
        {
            try
            {
                var includes = new string[] { "Patient", "Pharmacy", "Counselor", "StatusUpdatedBy" };

                var query = _proposalRepository.Query();

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                if (patientId.HasValue && patientId.Value != Guid.Empty)
                {
                    query = query.Where(p => p.PatientId == patientId.Value);
                }

                if (ids?.Any() == true)
                {
                    var statusValues = ids.Select(id => (Status)id).ToList();
                    query = query.Where(p => p.Status.HasValue && statusValues.Contains(p.Status.Value));
                }

                var proposals = await query.ToListAsync();

                return proposals.ToProposalBulkResponseDtoList();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve proposals", ex);
            }
        }

        public async Task<ProposalResponseDto?> GetProposalById(Guid proposalId)
        {


            var includes = new[] { "ProposalDetails", "ProposalDetails.Product", "Pharmacy", "Physician" };

            var proposal = await _proposalRepository.GetWithIncludeAsync(proposalId, includes);

            if (proposal == null) return null;

            return proposal.ToProposalResponseDto();
        }


        public async Task<CommonOperationResponseDto<Guid>> CreateProposalFromDtoAsync(ProposalRequestDto dto, int userId)
        {
            try
            {
                decimal? originalShippingPrice = null;

                if (dto.PharmacyShippingMethodId.HasValue)
                {
                    originalShippingPrice = await _pharmacyShippingMethodService
                        .GetShippingMethodPriceAsync(dto.PharmacyShippingMethodId.Value);
                }

                var proposal = new Proposal
                {
                    Name = dto.Name,
                    PatientId = dto.PatientId,
                    PharmacyId = dto.PharmacyId,
                    CounselorId = dto.CounselorId,
                    PhysicianId = dto.PhyisianId,
                    CouponId = dto.CouponId,
                    PatientCreditCardId = dto.PatientCreditCardId,
                    PharmacyShippingMethodId = dto.PharmacyShippingMethodId, 
                    ShippingAddressId = dto.ShippingAddressId,
                    IsAddressVerified = dto.IsAddressVerified ?? false,
                    StatusUpdatedById = userId,
                    TherapyExpiration = dto.TherapyExpiration,
                    Subtotal = dto.Subtotal,
                    TotalAmount = dto.TotalAmount,
                    CouponDiscount = dto.CouponDiscount,
                    Surcharge = dto.Surcharge,
                    DeliveryCharge = dto.DeliveryCharge,
                    Status = (Status)dto.Status,
                    Description = dto.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    IsActive = true,

                    IsDeliveryChargeOverRidden = dto.PharmacyShippingMethodId.HasValue &&
                                                originalShippingPrice.HasValue &&
                                                dto.DeliveryCharge != originalShippingPrice.Value
                };

                var proposalResponse = await _proposalRepository.AddAsync(proposal);
                //Default Credit Card Select
                if (dto.PatientCreditCardId.HasValue)
                {
                    var patientId = dto.PatientId;
                    var selectedCardId = dto.PatientCreditCardId.Value;

                    var existingDefaultCard = await _patientCreditCardRepository
                        .GetSingleAsync(x => x.PatientId == patientId && x.IsDefaultCreditCard == true && x.IsActive);

                    if (existingDefaultCard == null || existingDefaultCard.Id != selectedCardId)
                    {
                        var allCards = await _patientCreditCardRepository
                            .FindAsync(x => x.PatientId == patientId && x.IsActive);

                        foreach (var card in allCards)
                        {
                            card.IsDefaultCreditCard = (card.Id == selectedCardId);
                            card.UpdatedBy = userId.ToString();
                            card.UpdatedAt = DateTime.UtcNow;
                        }

                        await _patientCreditCardRepository.BulkUpdateAsync(allCards.ToList());
                    }
                }
                var detailsResponse = await _proposalDetailService.CreateProposalDetailAsync(dto.ProposalDetails, proposalResponse.Id, userId);
                await _proposalRepository.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid>
                {
                    Id = proposalResponse.Id,
                    Message = detailsResponse.FailedCount == 0
                        ? "Proposal and all details created successfully."
                        : "Proposal created, but some details failed."
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<BulkOperationResponseDto> UpdateProposalAsync(Guid id, ProposalRequestDto dto, int userId)
        {
            var response = new BulkOperationResponseDto();

            try
            {
                var existingProposal = await _proposalRepository.GetByIdAsync(id);
                if (existingProposal == null)
                {
                    response.SuccessCount = 0;
                    response.FailedCount = 1;
                    response.FailedIds.Add(id.ToString());
                    response.Message = "Proposal not found.";
                    return response;
                }

                decimal? originalShippingPrice = null;

                if (dto.PharmacyShippingMethodId.HasValue)
                {
                    originalShippingPrice = await _pharmacyShippingMethodService
                        .GetShippingMethodPriceAsync(dto.PharmacyShippingMethodId.Value);
                }

                existingProposal.Name = dto.Name;
                existingProposal.PatientId = dto.PatientId;
                existingProposal.PharmacyId = dto.PharmacyId;
                existingProposal.CounselorId = dto.CounselorId;
                existingProposal.CouponId = dto.CouponId;
                existingProposal.PatientCreditCardId = dto.PatientCreditCardId;
                existingProposal.PharmacyShippingMethodId = dto.PharmacyShippingMethodId;
                existingProposal.ShippingAddressId = dto.ShippingAddressId;
                existingProposal.IsAddressVerified = dto.IsAddressVerified ?? false;
                existingProposal.TherapyExpiration = dto.TherapyExpiration;
                existingProposal.Subtotal = dto.Subtotal;
                existingProposal.TotalAmount = dto.TotalAmount;
                existingProposal.CouponDiscount = dto.CouponDiscount;
                existingProposal.Surcharge = dto.Surcharge;
                existingProposal.DeliveryCharge = dto.DeliveryCharge;
                existingProposal.Status = (Status)dto.Status;
                existingProposal.Description = dto.Description;
                existingProposal.UpdatedAt = DateTime.UtcNow;
                existingProposal.UpdatedBy = userId.ToString();

                existingProposal.IsDeliveryChargeOverRidden = dto.PharmacyShippingMethodId.HasValue &&
                                                             originalShippingPrice.HasValue &&
                                                             dto.DeliveryCharge != originalShippingPrice.Value;

                var proposalResponse = await _proposalRepository.UpdateAsync(existingProposal);
                
                //Default Credit Card Select
                if (dto.PatientCreditCardId.HasValue)
                {
                    var patientId = dto.PatientId;
                    var selectedCardId = dto.PatientCreditCardId.Value;

                    var existingDefaultCard = await _patientCreditCardRepository
                        .GetSingleAsync(x => x.PatientId == patientId && x.IsDefaultCreditCard == true && x.IsActive);

                    if (existingDefaultCard == null || existingDefaultCard.Id != selectedCardId)
                    {
                        var allCards = await _patientCreditCardRepository
                            .FindAsync(x => x.PatientId == patientId && x.IsActive);

                        foreach (var card in allCards)
                        {
                            card.IsDefaultCreditCard = (card.Id == selectedCardId);
                            card.UpdatedBy = userId.ToString();
                            card.UpdatedAt = DateTime.UtcNow;
                        }

                        await _patientCreditCardRepository.BulkUpdateAsync(allCards.ToList());
                    }
                }

                var detailsResponse = await _proposalDetailService.UpdateProposalDetailsAsync(dto.ProposalDetails, id, userId);

                await _proposalRepository.SaveChangesAsync();

                response.SuccessCount = 1 + detailsResponse.SuccessCount;
                response.FailedCount = detailsResponse.FailedCount;
                response.SuccessIds.Add(proposalResponse.Id.ToString());
                response.SuccessIds.AddRange(detailsResponse.SuccessIds);
                response.FailedIds.AddRange(detailsResponse.FailedIds);
                response.Message = detailsResponse.FailedCount == 0
                    ? "Proposal and all details updated successfully."
                    : "Proposal updated, but some details failed.";
            }
            catch (Exception ex)
            {
                response.SuccessCount = 0;
                response.FailedCount = 1 + (dto.ProposalDetails?.Count ?? 0);
                response.FailedIds.Add(id.ToString());
                if (dto.ProposalDetails != null)
                {
                    response.FailedIds.AddRange(dto.ProposalDetails.Select(d => d.ProductId.ToString()));
                }
                response.Message = $"Failed to update proposal. Error: {ex.Message}";
            }

            return response;
        }

        public async Task<BulkOperationResponseDto> BulkDeleteProposalAsync(IList<Guid> proposalIds)
        {
            if (proposalIds == null || !proposalIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No valid proposal IDs provided."
                };
            }

            var proposalsToDelete = (await _proposalRepository.FindAsync(p => proposalIds.Contains(p.Id), noTracking: false)).ToList();
            if (!proposalsToDelete.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = proposalIds.Count,
                    Message = "No proposals found for the provided IDs."
                };
            }
            await _proposalRepository.RemoveRangeAsync(proposalsToDelete);
            await _proposalRepository.SaveChangesAsync();

            var successCount = proposalsToDelete.Count;
            var failedCount = proposalIds.Count - successCount;

            return new BulkOperationResponseDto
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                Message = $"{successCount} proposal(s) deleted successfully."
            };
        }
        public async Task<CommonOperationResponseDto<int>> BulkAssignProposalAsync(IEnumerable<Guid> proposalIds, int assigneeId, int userId)
        {
            if (proposalIds == null || !proposalIds.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No valid Proposal IDs provided."
                };
            }

            var proposalToAssign = (await _proposalRepository
                .FindAsync(p => proposalIds.Contains(p.Id), noTracking: false))
                .ToList();

            if (proposalIds == null || !proposalIds.Any())
            {
                return new CommonOperationResponseDto<int>
                {
                    Id = 0,
                    Message = "No proposals found for the provided IDs."
                };
            }

            foreach (var patient in proposalToAssign)
            {
                patient.CounselorId = assigneeId;
                patient.UpdatedBy = userId.ToString();
                patient.UpdatedAt = DateTime.UtcNow;
            }

            await _proposalRepository.BulkUpdateAsync(proposalToAssign);

            return new CommonOperationResponseDto<int>
            {
                Id = proposalToAssign.Count,
                Message = $"{proposalToAssign.Count} proposal(s) successfully assigned."
            };
        }
        public async Task<CommonOperationResponseDto<Guid>> UpdateProposalStatusAsync(Guid proposalId, int status, string? description, int userId)
        {
            var includes = new List<string>{ "ProposalDetails" };
            if((Status)status == Status.Approved)
            {
                includes.Add("Patient");
            }
            var proposal = await _proposalRepository.GetWithIncludeAsync(proposalId, includes.ToArray());

            if (proposal == null)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "No proposals found for the provided ID."
                };
            }

            proposal.StatusUpdatedById = userId;

            if (!string.IsNullOrWhiteSpace(description))
            {
                if (string.IsNullOrEmpty(proposal.Description))
                {
                    proposal.Description = $"Rejection Reason: {description}";
                }
                else
                {
                    proposal.Description += Environment.NewLine;
                    proposal.Description += $"Rejection Reason: {description}";
                }
            }

            proposal.Status = (Status)status;
            proposal.UpdatedBy = userId.ToString();
            proposal.UpdatedAt = DateTime.UtcNow;

            await _proposalRepository.UpdateAsync(proposal);

            if (proposal.Status == Status.Approved)
            {
                var orderRequestDto = proposal.ToOrder();
                var response = await _orderService.CreateOrderAsync(orderRequestDto, userId);
                return response;
            }

            return new CommonOperationResponseDto<Guid>
            {
                Id = proposal.Id,
                Message = "Proposal status modified successfully."
            };
        }

        public async Task<CommonOperationResponseDto<Guid>> CloneOrderToProposalAsync(Guid orderId, int userId)
        {
            var order = await _orderService.GetOrderByIdForProposalAsync(orderId);
            if (order == null)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "Order not found."
                };
            }

            var proposalRequest = order.ToProposalRequestDto();

            // Filter ProposalDetails based on active ProductPharmacyPriceListItem and Product
            var validProposalDetails = new List<ProposalDetailRequestDto>();
            var removedProductNames = new List<string>();

            foreach (var detail in proposalRequest.ProposalDetails)
            {
                var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductPharmacyPriceListItemId == detail.ProductPharmacyPriceListItemId);
                if (orderDetail == null)
                {
                    removedProductNames.Add($"Unknown product with PriceListItemId {detail.ProductPharmacyPriceListItemId}");
                    continue;
                }

                bool isPriceListItemActive = orderDetail.ProductPharmacyPriceListItem?.IsActive ?? false;
                bool isProductActive = orderDetail.Product?.IsActive ?? false;

                if (isPriceListItemActive && isProductActive)
                {
                    validProposalDetails.Add(detail);
                }
                else
                {
                    removedProductNames.Add(orderDetail.Product?.Name ?? $"Unknown product with PriceListItemId {detail.ProductPharmacyPriceListItemId}");
                }
            }

            if (!validProposalDetails.Any())
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "Cannot create proposal. No active products in order details."
                };
            }

            proposalRequest.ProposalDetails = validProposalDetails;

            var response = await CreateProposalFromDtoAsync(proposalRequest, userId);

            string message = response.Message ?? "Success";

            if (removedProductNames.Any())
            {
                message += " The following inactive products were removed: " + string.Join(", ", removedProductNames);
            }

            return new CommonOperationResponseDto<Guid>
            {
                Id = response.Id,
                Message = message
            };
        }

        public async Task<CommonOperationResponseDto<Guid>> UpdateProposalStatusToRejectByPatientAsync(Guid proposalId, int status, string? description, int userId)
        {
            var includes = new List<string> { "ProposalDetails" };
            if ((Status)status == Status.Approved)
            {
                includes.Add("Patient");
            }
            var proposal = await _proposalRepository.GetWithIncludeAsync(proposalId, includes.ToArray());

            if (proposal == null)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "No proposals found for the provided ID."
                };
            }

            proposal.StatusUpdatedById = userId;

            if (!string.IsNullOrWhiteSpace(description))
            {
                if (string.IsNullOrEmpty(proposal.Description))
                {
                    proposal.Description = $"Patient Rejection Reason: {description}";
                }
                else
                {
                    proposal.Description += Environment.NewLine;
                    proposal.Description += $"Patient Rejection Reason: {description}";
                }
            }

            proposal.Status = (Status)status;
            proposal.UpdatedBy = userId.ToString();
            proposal.UpdatedAt = DateTime.UtcNow;

            await _proposalRepository.UpdateAsync(proposal);

            if (proposal.Status == Status.Approved)
            {
                var orderRequestDto = proposal.ToOrder();
                var response = await _orderService.CreateOrderAsync(orderRequestDto, userId);
                return response;
            }

            return new CommonOperationResponseDto<Guid>
            {
                Id = proposal.Id,
                Message = "Proposal status modified successfully."
            };
        }
    }
}
