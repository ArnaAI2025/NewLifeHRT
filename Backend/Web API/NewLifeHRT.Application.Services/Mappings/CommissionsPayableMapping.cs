using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CommissionsPayableMapping
    {
        public static CommissionsPayableResponseDto ToCommissionsPayableResponse(this CommissionsPayable entity)
        {
            var order = entity.Order;
            var patient = order?.Patient;
            var pharmacy = order?.Pharmacy;

            return new CommissionsPayableResponseDto
            {
                PoolId = entity.PoolDetail?.PoolId ?? Guid.Empty,
                CommissionsPayableId = entity.Id,
                CommissionsPayableDetailId = Guid.Empty, 
                CommissionsPayableAmount = entity.CommissionPayable,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : string.Empty,
                PharmacyName = pharmacy?.Name ?? string.Empty,
                TotalSales = order?.Subtotal ?? 0m,
                EntryType = entity.EntryType.HasValue ? entity.EntryType.Value.ToString() : null,
                IsActive = entity.IsActive
            };
        }

        public static List<CommissionsPayableResponseDto> ToCommissionsPayableResponseList(this IEnumerable<CommissionsPayable> entities)
        {
            return entities?.Select(e => e.ToCommissionsPayableResponse()).ToList() ?? new List<CommissionsPayableResponseDto>();
        }
        public static CommissionsPayableDetailResponseDto? ToCommissionsPayableDetailResponse(this CommissionsPayable entity)
        {
            if (entity == null) return null;

            var order = entity.Order;
            var patient = order?.Patient;
            var pharmacy = order?.Pharmacy;

            string? counselorName = null;
            if (order?.Counselor != null)
            {
                var c = order.Counselor;
                counselorName = string.Join(' ', new[] { c.FirstName, c.LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
            }

            bool? anyDetailOverridden = null;
            if (order?.OrderDetails != null && order.OrderDetails.Count > 0)
            {
                bool seenTrue = order.OrderDetails.Any(od => od.IsPriceOverRidden == true);
                bool seenFalse = order.OrderDetails.Any(od => od.IsPriceOverRidden == false);
                anyDetailOverridden = seenTrue ? true : (seenFalse ? false : (bool?)null);
            }

            bool? deliveryOverridden = order?.IsDeliveryChargeOverRidden;

            bool? combinedOverride = null;
            if (anyDetailOverridden == true || deliveryOverridden == true) combinedOverride = true;
            else if (anyDetailOverridden == false || deliveryOverridden == false) combinedOverride = false;
            else combinedOverride = null;

            string? weekSummary = null;
            var pool = entity.PoolDetail?.Pool;
            if (pool != null)
            {
                string from = pool.FromDate.ToString("dd-MM-yyyy");
                string to = pool.ToDate.ToString("dd-MM-yyyy");
                weekSummary = $"{counselorName ?? string.Empty} {from} to {to} - W{pool.Week}".Trim();
            }

            var subtotal = order?.Subtotal;
            var delivery = order?.DeliveryCharge;
            var surcharge = order?.Surcharge;
            var syringe = entity.SyringeCost;

            var commissionAppliedTotal = entity.CommissionBaseAmount;
            var commissionPayable = entity.CommissionPayable;

            var orderTotal = order?.TotalAmount;
            var pharmacyName = pharmacy?.Name;
            var ctc = entity.CTC;
            var ctcPlusCommission = entity.CTC + entity.CommissionPayable;
            var profitAmount = entity.FinancialResult;

            decimal? netAmount = null;
            if (subtotal.HasValue)
            {
                netAmount = subtotal.Value - (entity.CTC + entity.CommissionPayable);
            }

            var commissionPayStatus = entity.EntryType?.ToString();

            var patientName = patient != null
                ? string.Join(' ', new[] { patient.FirstName, patient.LastName }.Where(s => !string.IsNullOrWhiteSpace(s)))
                : null;

            return new CommissionsPayableDetailResponseDto
            {
                CounselorName = counselorName ?? string.Empty,
                CommissionPayStatus = commissionPayStatus,
                WeekSummary = weekSummary,
                PatientName = patientName,
                OrdersName = order?.Name,
                SubTotalAmount = subtotal,
                Shipping = delivery,
                Surcharge = surcharge,
                Syringe = syringe,
                CommissionAppliedTotal = commissionAppliedTotal,
                CommissionPayable = commissionPayable,
                TotalAmount = orderTotal,
                Discount = order?.CouponDiscount,
                PharmacyName = pharmacyName,
                CommissionCalculationDetails = entity.CommissionCalculationDetails,
                CtcCalculationDetails = entity.CtcCalculationDetails,
                Ctc = ctc,
                CtcPlusCommission = ctcPlusCommission,
                ProfitAmount = profitAmount,
                NetAmount = netAmount,
                IsPriceOverRidden = combinedOverride,
                PoolDetailId = entity.PoolDetail?.Id,
                IsMissingProductPrice = entity.IsMissingProductPrice,
            };
        }


    }
}
