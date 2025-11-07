using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.DTOs;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class ProposalDetailService : IProposalDetailService
    {
        private readonly IProposalDetailRepository _proposalDetailRepository;
        private readonly IPriceListItemService _priceListItemService;
        public ProposalDetailService(IProposalDetailRepository proposalDetailRepository, IPriceListItemService priceListItemService) {
        _proposalDetailRepository = proposalDetailRepository;
            _priceListItemService = priceListItemService;
        }
        public async Task<BulkOperationResponseDto> CreateProposalDetailAsync(
    List<ProposalDetailRequestDto> dtoList, Guid proposalId, int userId)
        {
            var response = new BulkOperationResponseDto();

            try
            {
                var priceListItemIds = dtoList.Select(d => d.ProductPharmacyPriceListItemId).ToList();

                var priceMap = await _priceListItemService.GetPricesByIdsAsync(priceListItemIds);
                var proposalDetails = dtoList.Select(dto =>
                {
                    var originalPrice = priceMap[dto.ProductPharmacyPriceListItemId];
                    bool isOverridden = dto.Amount != originalPrice;

                    return new ProposalDetail
                    {
                        ProductPharmacyPriceListItemId = dto.ProductPharmacyPriceListItemId,
                        ProposalId = proposalId,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        IsPriceOverRidden = isOverridden,
                        Amount = dto.Amount,
                        Protocol = dto.Protocol,
                        PerUnitAmount = dto.PerUnitAmount,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId.ToString(),
                    };
                }).ToList();

                await _proposalDetailRepository.AddRangeAsync(proposalDetails);

                response.SuccessCount = proposalDetails.Count;
                response.FailedCount = 0;
                response.Message = "All proposal details inserted successfully.";
                response.SuccessIds = proposalDetails.Select(pd => pd.ProductId.ToString()).ToList();
            }
            catch (Exception ex)
            {
                response.SuccessCount = 0;
                response.FailedCount = dtoList.Count;
                response.Message = $"Failed to insert proposal details. Error: {ex.Message}";
                response.FailedIds = dtoList.Select(d => d.ProductId.ToString()).ToList();
            }

            return response;
        }

        public async Task<BulkOperationResponseDto> UpdateProposalDetailsAsync(List<ProposalDetailRequestDto> dtoList, Guid proposalId, int userId)
        {
            var response = new BulkOperationResponseDto();

            try
            {
                // Get existing proposal details
                var existingDetails = await _proposalDetailRepository.FindAsync(pd => pd.ProposalId == proposalId);
                var existingDetailsList = existingDetails.ToList();

                // Remove existing details that are not in the new list
                var incomingProductIds = dtoList.Select(d => d.ProductPharmacyPriceListItemId).ToHashSet();
                var detailsToRemove = existingDetailsList.Where(ed => !incomingProductIds.Contains(ed.ProductPharmacyPriceListItemId)).ToList();

                if (detailsToRemove.Any())
                {
                    await _proposalDetailRepository.RemoveRangeAsync(detailsToRemove);
                }

                var incomingPriceListItemIds = dtoList
                    .Select(d => d.ProductPharmacyPriceListItemId)
                    .ToList();

                var priceMap = await _priceListItemService.GetPricesByIdsAsync(incomingPriceListItemIds);

                var successfulOperations = new List<string>();
                var failedOperations = new List<string>();

                foreach (var dto in dtoList)
                {
                    try
                    {
                        var existingDetail = existingDetailsList.FirstOrDefault(ed => ed.ProductPharmacyPriceListItemId == dto.ProductPharmacyPriceListItemId);

                        decimal originalPrice = priceMap.TryGetValue(dto.ProductPharmacyPriceListItemId, out var price)
                            ? price
                            : (decimal)dto.Amount; 

                        if (existingDetail != null)
                        {
                            // Update existing detail
                            existingDetail.Quantity = dto.Quantity;
                            existingDetail.Amount = dto.Amount;
                            existingDetail.PerUnitAmount = dto.PerUnitAmount;
                            existingDetail.Protocol = dto.Protocol;
                            existingDetail.UpdatedAt = DateTime.UtcNow;
                            existingDetail.UpdatedBy = userId.ToString();

                            existingDetail.IsPriceOverRidden = dto.Amount != originalPrice;

                            await _proposalDetailRepository.UpdateAsync(existingDetail);
                            successfulOperations.Add(dto.ProductId.ToString());
                        }
                        else
                        {
                            // Create new detail
                            var newDetail = new ProposalDetail
                            {
                                ProposalId = proposalId,
                                ProductPharmacyPriceListItemId = dto.ProductPharmacyPriceListItemId,
                                ProductId = dto.ProductId,
                                Quantity = dto.Quantity,
                                Amount = dto.Amount,
                                PerUnitAmount = dto.PerUnitAmount,
                                Protocol = dto.Protocol,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = userId.ToString(),

                                IsPriceOverRidden = dto.Amount != originalPrice
                            };

                            await _proposalDetailRepository.AddAsync(newDetail);
                            successfulOperations.Add(dto.ProductId.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        failedOperations.Add(dto.ProductId.ToString());
                        // Log individual detail error if needed
                    }
                }

                response.SuccessCount = successfulOperations.Count;
                response.FailedCount = failedOperations.Count;
                response.SuccessIds = successfulOperations;
                response.FailedIds = failedOperations;
                response.Message = failedOperations.Count == 0
                    ? "All proposal details updated successfully."
                    : $"{successfulOperations.Count} details updated successfully, {failedOperations.Count} failed.";
            }
            catch (Exception ex)
            {
                response.SuccessCount = 0;
                response.FailedCount = dtoList.Count;
                response.Message = $"Failed to update proposal details. Error: {ex.Message}";
                response.FailedIds = dtoList.Select(d => d.ProductId.ToString()).ToList();
            }

            return response;
        }

        public async Task<BulkOperationResponseDto> BulkToggleActiveStatusAsync(IList<Guid> proposalIds, int userId, bool isActive)
        {
            if (proposalIds == null || !proposalIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No valid proposal detail IDs provided."
                };
            }

            var proposalDetailsToUpdate = (await _proposalDetailRepository
                .FindAsync(pd => proposalIds.Contains(pd.ProposalId), noTracking: false))
                .ToList();

            if (!proposalDetailsToUpdate.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = proposalIds.Count,
                    Message = "No proposal details found for the provided IDs."
                };
            }

            foreach (var proposalDetail in proposalDetailsToUpdate)
            {
                proposalDetail.IsActive = isActive;
                proposalDetail.UpdatedBy = userId.ToString();
                proposalDetail.UpdatedAt = DateTime.UtcNow;
            }

            await _proposalDetailRepository.BulkUpdateAsync(proposalDetailsToUpdate);
            await _proposalDetailRepository.SaveChangesAsync(); 

            var successCount = proposalDetailsToUpdate.Count;
            var failedCount = proposalIds.Count - successCount;

            return new BulkOperationResponseDto
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                SuccessIds = proposalDetailsToUpdate.Select(pd => pd.Id.ToString()).ToList(),
                FailedIds = proposalIds.Where(id => !proposalDetailsToUpdate.Any(pd => pd.Id == id)).Select(id => id.ToString()).ToList(),
                Message = isActive? $"{successCount} proposal detail(s) activated successfully.": $"{successCount} proposal detail(s) deactivated successfully."
            };
        }





    }
}
