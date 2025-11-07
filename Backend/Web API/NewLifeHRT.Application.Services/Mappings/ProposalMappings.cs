using NewLifeHRT.Api.Requests;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.DTOs;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProposalMappings
    {
        public static ProposalResponseDto ToProposalResponseDto(this Proposal proposal)
        {
            return new ProposalResponseDto
            {
                Id = proposal.Id,
                Name = proposal.Name,
                PatientId = proposal.PatientId,
                PharmacyId = proposal.PharmacyId,
                CounselorId = proposal.CounselorId,
                Physician = new DropDownIntResponseDto
                {
                    Id = (int)proposal.PhysicianId,
                    Value = $"{proposal.Physician?.FirstName ?? ""} {proposal.Physician?.LastName ?? ""}".Trim()
                },

                CouponId = proposal.CouponId,
                PatientCreditCardId = proposal.PatientCreditCardId,
                PharmacyShippingMethodId = proposal.PharmacyShippingMethodId,
                ShippingAddressId = proposal.ShippingAddressId,
                TherapyExpiration = proposal.TherapyExpiration,
                Subtotal = proposal.Subtotal,
                TotalAmount = proposal.TotalAmount,
                Surcharge = proposal.Surcharge,
                CouponDiscount = proposal.CouponDiscount,
                DeliveryCharge = proposal.DeliveryCharge,
                IsLab = (bool)proposal?.Pharmacy?.IsLab,
                Status = (int?)proposal.Status,
                IsAddressVerified = proposal.IsAddressVerified,
                Description = proposal.Description,

                ProposalDetails = proposal.ProposalDetails?.Select(pd => new ProposalDetailResponseDto
                {
                    Id = pd.Id,
                    ProductPharmacyPriceListItemId = pd.ProductPharmacyPriceListItemId,
                    ProposalId = pd.ProposalId,
                    ProductId = pd.ProductId,
                    Quantity = pd.Quantity,
                    Amount = pd.Amount,
                    PerUnitAmount = pd.PerUnitAmount,
                     IsColdStorageProduct = pd.Product?.IsColdStorageProduct,
                    ProductName = pd.Product?.Name,
                    Protocol = pd.Protocol,
                }).ToList() ?? new List<ProposalDetailResponseDto>()
            };
        }

        public static List<ProposalBulkResponseDto> ToProposalBulkResponseDtoList(this IEnumerable<Proposal> proposals)
        {
            return proposals.Select(p => p.ToProposalBulkResponseDto()).ToList();
        }

        public static ProposalBulkResponseDto ToProposalBulkResponseDto(this Proposal proposal)
        {
            return new ProposalBulkResponseDto
            {
                Id = proposal.Id,
                Name = proposal.Name ?? string.Empty,
                PatientName = $"{proposal.Patient?.FirstName ?? ""} {proposal.Patient?.LastName ?? ""}".Trim(),
                PharmacyName = proposal.Pharmacy?.Name ?? string.Empty,
                IsActive = proposal.IsActive,
                Status = (int?)proposal.Status,
                CounselorName = $"{proposal.Counselor?.FirstName ?? ""} {proposal.Counselor?.LastName ?? ""}".Trim(),
                StatusUpdatedByName = $"{proposal.StatusUpdatedBy?.FirstName ?? ""} {proposal.StatusUpdatedBy?.LastName ?? ""}".Trim(),
                TherapyExpiration = proposal.TherapyExpiration,
                CreatedAt = proposal.CreatedAt,
            };
        }
        public static ProposalRequestDto ToProposalRequestDto(this Order orderDto)
        {
            if (orderDto == null) return null;

            return new ProposalRequestDto
            {
                Name = orderDto.Name,
                PatientId = orderDto.PatientId,
                PharmacyId = orderDto.PharmacyId,
                CounselorId = orderDto.CounselorId,
                CouponId = orderDto.CouponId,
                PatientCreditCardId = orderDto.PatientCreditCardId,
                PharmacyShippingMethodId = orderDto.PharmacyShippingMethodId,
                ShippingAddressId = orderDto.ShippingAddressId,
                TherapyExpiration = orderDto.TherapyExpiration,
                Subtotal = orderDto.Subtotal,
                TotalAmount = orderDto.TotalAmount,
                CouponDiscount = orderDto.CouponDiscount,
                Surcharge = orderDto.Surcharge,
                DeliveryCharge = orderDto.DeliveryCharge,
                Status = orderDto.Status.HasValue ? (int)Status.Draft : (int)Status.Draft,
                Description = orderDto.Description,        
                PhyisianId = (int)orderDto.PhysicianId,
                ProposalDetails = orderDto.OrderDetails?.Select(od => new ProposalDetailRequestDto
                {
                    ProductId = od.ProductId,
                    ProductPharmacyPriceListItemId = od.ProductPharmacyPriceListItemId,
                    Quantity = od.Quantity,
                    PerUnitAmount = od.PerUnitAmount,
                    Amount = od.Amount ?? 0
                }).ToList() ?? new List<ProposalDetailRequestDto>(),
            };
        }

    }
}
