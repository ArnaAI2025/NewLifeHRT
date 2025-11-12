using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Helpers
{
    public class CommissionCalculatorHelper
    {
        // Returns the UTC date range of the week from Friday to Thursday for a given date
        public static (DateTime From, DateTime To) GetWeekRangeFriThuUtc(DateTime todayUtc)
        {
            var today = todayUtc.Date;
            int diffToFriday = ((int)today.DayOfWeek - (int)DayOfWeek.Friday + 7) % 7;
            var from = today.AddDays(-diffToFriday);
            var to = from.AddDays(6);
            return (from, to);
        }

        // Calculates commissions payable for an order, returns null if invalid input
        public static CommissionsPayable? CalculateCommission(Order order)
        {
            if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
                return null;

            var sb = new StringBuilder();
            var ctcSb = new StringBuilder();

            var pharmacy = order.Pharmacy;
            var counselor = order.Counselor;

            if (pharmacy == null || counselor == null)
                return null;

            // Base amount for commission excludes product type with TypeId 5 (e.g. syringes)
            decimal commissionBaseAmount = order.Subtotal
                - order.OrderDetails.Where(d => d.Product?.TypeId == 5).Sum(d => d.Amount ?? 0m);

            var commissionsPayable = new CommissionsPayable
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                CommissionBaseAmount = commissionBaseAmount,
                CommissionsPayablesDetails = new List<CommissionsPayablesDetail>(),
                SyringeCost = 0m,
                IsActive = true
            };

            decimal totalCommission = 0m;
            decimal totalCtc = 0m;
            decimal totalFinancialResult = 0m;
            decimal totalSyringeCost = 0m;
            var shippingMethodName = order.PharmacyShippingMethod?.ShippingMethod?.Name ?? "N/A";
            decimal salesPersonCommissionPercent = counselor.CommisionInPercentage ?? 0m;

            bool isAnyProductMissingPrice = false; // track if any product cost data missing

            // Build cost to company (CTC) and financial results details string
            foreach (var detail in order.OrderDetails)
            {
                string productName = detail.Product?.Name ?? string.Empty;

                bool missingPrice = detail.ProductPharmacyPriceListItem == null || detail.ProductPharmacyPriceListItem.CostOfProduct == null;
                if (missingPrice)
                {
                    isAnyProductMissingPrice = true;
                }

                decimal costPerUnit = (decimal)(detail.ProductPharmacyPriceListItem?.CostOfProduct ?? 0m);
                decimal totalCost = costPerUnit * detail.Quantity;
                decimal saleAmount = (decimal)(detail.Amount ?? 0m);

                decimal financialResult = saleAmount - totalCost;
                string profitLabel = financialResult >= 0 ? "Profit" : "Loss";

                // Append details about product pricing and profit/loss
                ctcSb.AppendLine($"Product: {productName}");
                ctcSb.AppendLine($"Price: {detail.PerUnitAmount}");
                ctcSb.AppendLine($"Quantity: {detail.Quantity}");
                ctcSb.AppendLine($"IsPriceOverridden: {detail.IsPriceOverRidden ?? false}");
                ctcSb.AppendLine($"Cost/Unit: {costPerUnit:N2}");
                ctcSb.AppendLine($"Total Cost: {totalCost:N2}");
                ctcSb.AppendLine($"Sale Amount: {saleAmount:N2}");
                ctcSb.AppendLine($"{profitLabel}: {financialResult:N2}");
                ctcSb.AppendLine();

                totalCtc += totalCost;
                totalFinancialResult += financialResult;

                if (detail.Product?.TypeId == 5) // Syringe type products
                    totalSyringeCost += saleAmount;
            }

            // Summary lines for CTC details
            ctcSb.AppendLine($"Total Sale: {order.Subtotal:N2}");
            ctcSb.AppendLine($"Total Cost: {totalCtc:N2}");
            ctcSb.AppendLine($"Shipping Method Name: {shippingMethodName}");
            ctcSb.AppendLine($"Shipping Cost: {(order.DeliveryCharge ?? 0m):N2}");
            string totalProfitLabel = totalFinancialResult >= 0 ? "Profit" : "Loss";
            ctcSb.AppendLine($"{totalProfitLabel}: {totalFinancialResult:N2}");

            commissionsPayable.CtcCalculationDetails = ctcSb.ToString();

            // Parse replacement commission rate map if applicable
            Dictionary<decimal, decimal>? replacementMap = null;
            if (counselor.MatchAsCommisionRate.HasValue &&
                counselor.MatchAsCommisionRate == false &&
                !string.IsNullOrWhiteSpace(counselor.ReplaceCommisionRate))
            {
                replacementMap = ParseReplacementMap(counselor.ReplaceCommisionRate);
            }

            // Calculate commission per order detail
            foreach (var detail in order.OrderDetails)
            {
                sb.AppendLine($"<======================= {detail.Product?.Name} =======================>");
                sb.AppendLine($"Quantity: {detail.Quantity}, Price per unit: {detail.PerUnitAmount}");
                sb.AppendLine($"Product Amount: {detail.Amount}");

                decimal productAmount = (decimal)(detail.Amount ?? 0m);
                decimal ctc = (decimal)(detail.ProductPharmacyPriceListItem?.CostOfProduct ?? 0m) * detail.Quantity;
                decimal financialResult = productAmount - ctc;

                if (detail.Product?.TypeId == 5) // Syringe products don't earn commissions
                {
                    sb.AppendLine("[SYRINGE PRODUCT] Commission not applied.");
                    commissionsPayable.CommissionsPayablesDetails.Add(new CommissionsPayablesDetail
                    {
                        Id = Guid.NewGuid(),
                        CommissionsPayableId = commissionsPayable.Id,
                        OrderDetailId = detail.Id,
                        CTC = ctc,
                        FinancialResult = financialResult,
                        CommissionType = "SyringeProduct",
                        CommissionPercentage = "0.00",
                        CommissionPayable = 0m,
                        IsActive = true
                    });
                    continue;
                }

                decimal commissionPercentToApply = salesPersonCommissionPercent;

                // Fixed commission from pharmacy caps the sales person commission
                if (pharmacy.HasFixedCommission && pharmacy.CommissionPercentage.HasValue)
                {
                    commissionPercentToApply = Math.Min(pharmacy.CommissionPercentage.Value, salesPersonCommissionPercent);
                    sb.AppendLine($"[FIXED COMMISSION] Applying {commissionPercentToApply:N2}%");
                }
                else
                {
                    // Find applicable commission rate based on product amount range
                    var applicableRate = detail.Product?.CommisionRates?.FirstOrDefault(cr =>
                        productAmount >= cr.FromAmount &&
                        productAmount <= cr.ToAmount &&
                        cr.IsActive);

                    if (applicableRate != null && applicableRate.RatePercentage.HasValue)
                    {
                        decimal rangeRate = applicableRate.RatePercentage.Value;
                        decimal finalRate = rangeRate;

                        // Replace commission rate if mapping applies
                        if (replacementMap != null && replacementMap.TryGetValue(rangeRate, out var replacedRate))
                        {
                            finalRate = replacedRate;
                            sb.AppendLine($"[REPLACEMENT MAP] Original {rangeRate:N2}% replaced with {finalRate:N2}%");
                        }

                        // Apply the lower of salesperson's percent or the final rate
                        commissionPercentToApply = Math.Min(salesPersonCommissionPercent, finalRate);

                        if (commissionPercentToApply == finalRate)
                            sb.AppendLine($"[RANGE COMMISSION] comparing {applicableRate.FromAmount} > {productAmount} <= {applicableRate.ToAmount}   Applying range rate: {finalRate:N2}% ");
                        else
                            sb.AppendLine($"[SALES PERSON CAP] Salesperson commission: {commissionPercentToApply:N2}% for amount {productAmount:N2}");
                    }
                    else
                    {
                        sb.AppendLine($"[NO RATE MATCHED] Applying SalesPerson commission {salesPersonCommissionPercent:N2}%");
                    }
                }

                decimal commissionForDetail = (commissionPercentToApply / 100m) * productAmount;
                totalCommission += commissionForDetail;

                sb.AppendLine($"Calculated commission: {commissionForDetail:N2}");

                commissionsPayable.CommissionsPayablesDetails.Add(new CommissionsPayablesDetail
                {
                    Id = Guid.NewGuid(),
                    CommissionsPayableId = commissionsPayable.Id,
                    OrderDetailId = detail.Id,
                    CTC = ctc,
                    FinancialResult = financialResult,
                    CommissionType = commissionPercentToApply == salesPersonCommissionPercent ? "SalesPersonCommission" : "CommissionRateBased",
                    CommissionPercentage = commissionPercentToApply.ToString("N2"),
                    CommissionPayable = commissionForDetail,
                    IsActive = true
                });
            }

            // Append summary info for commission calculation
            sb.AppendLine($"Commission Base Amount (eligible): {commissionBaseAmount:N2}");
            sb.AppendLine($"Total Commission Applied: {totalCommission:N2}");
            sb.AppendLine($"Syringe Products Total Cost: {totalSyringeCost:N2}");
            sb.AppendLine($"Surcharge Cost: {(order.Surcharge ?? 0m):N2}");
            sb.AppendLine($"Shipping Method Name: {shippingMethodName}");
            sb.AppendLine($"Shipping Cost: {(order.DeliveryCharge ?? 0m):N2}");
            sb.AppendLine($"Total Amount: {order.TotalAmount:N2}");

            commissionsPayable.CommissionCalculationDetails = sb.ToString();
            commissionsPayable.CommissionPayable = totalCommission;
            commissionsPayable.CTC = totalCtc;
            commissionsPayable.FinancialResult = totalFinancialResult;
            commissionsPayable.SyringeCost = totalSyringeCost;

            commissionsPayable.IsMissingProductPrice = isAnyProductMissingPrice;

            return commissionsPayable;
        }

        // Parses a string of "original:replacement;..." commission rates into a dictionary mapping
        private static Dictionary<decimal, decimal> ParseReplacementMap(string replaceComRate)
        {
            var map = new Dictionary<decimal, decimal>();

            if (string.IsNullOrWhiteSpace(replaceComRate))
                return map;

            var pairs = replaceComRate.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var parts = pair.Split(':', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2 &&
                    decimal.TryParse(parts[0], out var orig) &&
                    decimal.TryParse(parts[1], out var replaced) &&
                    !map.ContainsKey(orig))
                {
                    map.Add(orig, replaced);
                }
            }

            return map;
        }
    }
}
