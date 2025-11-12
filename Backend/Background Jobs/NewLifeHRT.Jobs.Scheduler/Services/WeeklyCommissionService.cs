using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using System.Globalization;
using System.Text;
using CommissionReportDto = NewLifeHRT.Jobs.Scheduler.Models.CommissionReportDto;
using CommissionDetailDto = NewLifeHRT.Jobs.Scheduler.Models.CommissionDetailDto;
using CommissionSummaryDto = NewLifeHRT.Jobs.Scheduler.Models.CommissionSummaryDto;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public class WeeklyCommissionService : IWeeklyCommissionService
    {
        private readonly ClinicDbContext _clinicDbContext;

        public WeeklyCommissionService(ClinicDbContext clinicDbContext)
        {
            _clinicDbContext = clinicDbContext;
        }

        /// <summary>
        /// Calculates the UTC date range for a week that starts on Friday and ends on Thursday,
        /// based on the specified current UTC date.
        /// </summary>
        private static (DateTime From, DateTime To) GetWeekRangeFriThuUtc(DateTime todayUtc)
        {
            var today = todayUtc.Date;
            int diffToFriday = ((int)today.DayOfWeek - (int)DayOfWeek.Friday + 7) % 7;
            var from = today.AddDays(-diffToFriday);
            var to = from.AddDays(6);
            return (from, to);
        }

        /// <summary>
        /// Ensures that a <see cref="Pool"/> record exists for the specified date range and week.
        /// </summary>
        private async Task<Pool> EnsurePoolExistsAsync(DateTime fromDateUtc, DateTime toDateUtc)
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            int weekNumber = cal.GetWeekOfYear(
                fromDateUtc,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);

            var pool = await _clinicDbContext.Pools
                .FirstOrDefaultAsync(p =>
                    p.FromDate == fromDateUtc &&
                    p.ToDate == toDateUtc &&
                    p.Week == weekNumber);

            if (pool != null)
                return pool;

            pool = await _clinicDbContext.Pools
                .FirstOrDefaultAsync(p =>
                    p.Week == weekNumber &&
                    p.FromDate.Year == fromDateUtc.Year);

            if (pool == null)
            {
                pool = new Pool
                {
                    FromDate = fromDateUtc,
                    ToDate = toDateUtc,
                    Week = weekNumber,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _clinicDbContext.Pools.AddAsync(pool);
                await _clinicDbContext.SaveChangesAsync();

                await AddActiveCounselorsToPoolAsync(pool);
            }

            return pool;
        }

        /// <summary>
        /// Retrieves the current week's <see cref="Pool"/> and its associated <see cref="PoolDetail"/> records.
        /// </summary>
        public async Task<(Pool? Pool, List<PoolDetail> PoolDetails)> GetTodayPoolAndDetailsAsync()
        {
            var (from, to) = GetWeekRangeFriThuUtc(DateTime.UtcNow);
            var pool = await EnsurePoolExistsAsync(from, to);
            var details = await _clinicDbContext.PoolDetails
                .Where(pd => pd.PoolId == pool.Id)
                .ToListAsync();

            return (pool, details);
        }

        /// <summary>
        /// Retrieves all orders that are ready for commission generation within the current week,
        /// calculates their commission payables, and ensures related pool data consistency.
        /// </summary>
        public async Task<List<Order>> GetReadyToGenerateCommission()
        {
            var (from, to) = GetWeekRangeFriThuUtc(DateTime.UtcNow);
            var pool = await EnsurePoolExistsAsync(from, to);

            // Preload pool details
            var poolDetails = await _clinicDbContext.PoolDetails
                .Where(pd => pd.PoolId == pool.Id)
                .ToListAsync();

            var counselorIds = poolDetails.Select(pd => pd.CounselorId).ToList();

            var orders = await _clinicDbContext.Orders
                .Where(o => o.IsActive
                            && o.IsGenrateCommision == true
                            && o.CommissionGeneratedDate.HasValue
                            && o.CommissionGeneratedDate.Value.Date >= pool.FromDate
                            && o.CommissionGeneratedDate.Value.Date <= pool.ToDate
                            && counselorIds.Contains(o.CounselorId))
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductPharmacyPriceListItem)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.CommisionRates)
                .Include(o => o.Patient)
                .Include(o => o.Pharmacy)
                    .ThenInclude(p => p.PharmacyShippingMethods)
                        .ThenInclude(sm => sm.ShippingMethod)
                .Include(o => o.Counselor)
                .ToListAsync();

            foreach (var order in orders)
            {
                var commissionsPayable = CalculateCommission(order);
                if (commissionsPayable == null)
                    continue;

                var poolDetail = poolDetails.FirstOrDefault(pd => pd.CounselorId == order.CounselorId);
                if (poolDetail != null)
                    commissionsPayable.PoolDetailId = poolDetail.Id;

                if (!await _clinicDbContext.CommissionsPayables.AnyAsync(cp => cp.OrderId == commissionsPayable.OrderId
                    && cp.PoolDetailId == commissionsPayable.PoolDetailId
                    && cp.EntryType == commissionsPayable.EntryType))
                {
                    await InsertIntoCommissionPayablesAndCommissionPayableDetails(commissionsPayable);
                }

            }

            var nextFrom = from.AddDays(7);
            var nextTo = to.AddDays(7);
            await EnsurePoolExistsAsync(nextFrom, nextTo);

            return orders;
        }

        /// <summary>
        /// Inserts a <see cref="CommissionsPayable"/> record and its related
        /// <see cref="CommissionsPayablesDetail"/> entries into the database.
        /// </summary>
        public async Task<int> InsertIntoCommissionPayablesAndCommissionPayableDetails(CommissionsPayable commissionsPayable)
        {
            await _clinicDbContext.CommissionsPayables.AddAsync(commissionsPayable);

            if (commissionsPayable.CommissionsPayablesDetails != null &&
                commissionsPayable.CommissionsPayablesDetails.Any())
            {
                await _clinicDbContext.CommissionsPayablesDetails
                    .AddRangeAsync(commissionsPayable.CommissionsPayablesDetails);
            }

            return await _clinicDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves an order with all related entities and calculates its commission details.
        /// </summary>
        public async Task<CommissionsPayable?> GetFullOrderByIdAsync(Guid orderId)
        {
            var order = await _clinicDbContext.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductPharmacyPriceListItem)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.CommisionRates)
                .Include(o => o.Patient)
                .Include(o => o.Pharmacy)
                .Include(o => o.Counselor)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            return CalculateCommission(order);
        }

        /// <summary>
        /// Creates or retrieves the current week's active pool based on a Friday–Thursday range.
        /// </summary>
        public async Task<Pool> CreatePool()
        {
            var (from, to) = GetWeekRangeFriThuUtc(DateTime.UtcNow);
            return await EnsurePoolExistsAsync(from, to);
        }

        public async Task<List<ApplicationUser>> GetAllActiveCounselorsAsync()
        {
            const int counselorRoleId = 6;

            return await _clinicDbContext.Users
                .Where(u => !u.IsDeleted && u.RoleId == counselorRoleId)
                .ToListAsync();
        }

        /// <summary>
        /// Adds all currently active counselors to the specified pool,  
        /// ensuring no duplicate counselor entries already exist in that pool.
        /// </summary>
        public async Task AddActiveCounselorsToPoolAsync(Pool pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var activeCounselors = await GetAllActiveCounselorsAsync();

            var existingCounselorIds = await _clinicDbContext.PoolDetails
                .Where(pd => pd.PoolId == pool.Id)
                .Select(pd => pd.CounselorId)
                .ToListAsync();

            var newPoolDetails = activeCounselors
                .Where(c => !existingCounselorIds.Contains(c.Id))
                .Select(c => new PoolDetail
                {
                    PoolId = pool.Id,
                    CounselorId = c.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                })
                .ToList();

            if (newPoolDetails.Any())
            {
                await _clinicDbContext.PoolDetails.AddRangeAsync(newPoolDetails);
                await _clinicDbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Calculates the total commission payable for a given order, including detailed
        /// per-product commission breakdowns, cost-to-company (CTC) calculations, and
        /// financial results.
        /// </summary>
        public static CommissionsPayable? CalculateCommission(Order order)
        {
            // Validate input — must have order and order details to proceed
            if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
                return null;

            // String builders to hold textual logs for auditing and debugging
            var sb = new StringBuilder();
            var ctcSb = new StringBuilder();

            // Fetch associated entities
            var pharmacy = order.Pharmacy;
            var salesPerson = order.Counselor;

            // Stop if critical references are missing
            if (pharmacy == null || salesPerson == null)
                return null;

            // Determine base amount for commission by excluding syringe-type products (TypeId = 5)
            decimal commissionBaseAmount = order.Subtotal
                - order.OrderDetails.Where(d => d.Product?.TypeId == 5).Sum(d => d.Amount ?? 0m);

            // Initialize the main commission record
            var commissionsPayable = new CommissionsPayable
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                CommissionBaseAmount = commissionBaseAmount,
                CommissionsPayablesDetails = new List<CommissionsPayablesDetail>(),
                SyringeCost = 0m,
                IsActive = true
            };

            // Initialize totals for accumulation
            decimal totalCommission = 0m;
            decimal totalCtc = 0m;
            decimal totalFinancialResult = 0m;
            decimal totalSyringeCost = 0m;

            // Gather shipping and commission context
            var shippingMethodName = order.PharmacyShippingMethod?.ShippingMethod?.Name ?? "N/A";
            var costOfShipping = order.PharmacyShippingMethod?.CostOfShipping;
            decimal salesPersonCommissionPercent = salesPerson.CommisionInPercentage ?? 0m;

            // Flag to track any missing cost-of-product information
            bool isAnyProductMissingPrice = false;

            // ------------------- CTC (Cost-To-Company) Calculation -------------------
            // Loop through each product detail to compute cost, sale amount, and profit/loss
            foreach (var detail in order.OrderDetails)
            {
                string productName = detail.Product?.Name ?? string.Empty;

                bool missingPrice = detail.ProductPharmacyPriceListItem == null || detail.ProductPharmacyPriceListItem.CostOfProduct == null;
                if (missingPrice)
                {
                    isAnyProductMissingPrice = true;
                }

                // Basic per-product cost and profit computation
                decimal costPerUnit = (decimal)(detail.ProductPharmacyPriceListItem?.CostOfProduct ?? 0m);
                decimal totalCost = costPerUnit * detail.Quantity;
                decimal saleAmount = (decimal)(detail.Amount ?? 0m);
                decimal financialResult = saleAmount - totalCost;

                string profitLabel = financialResult >= 0 ? "Profit" : "Loss";

                // Log calculation details for auditing
                ctcSb.AppendLine($"Product: {productName}");
                ctcSb.AppendLine($"Price: {detail.PerUnitAmount}");
                ctcSb.AppendLine($"Quantity: {detail.Quantity}");
                ctcSb.AppendLine($"IsPriceOverridden: {detail.IsPriceOverRidden ?? false}");
                ctcSb.AppendLine($"Cost/Unit: {costPerUnit:N2}");
                ctcSb.AppendLine($"Total Cost: {totalCost:N2}");
                ctcSb.AppendLine($"Sale Amount: {saleAmount:N2}");
                ctcSb.AppendLine($"{profitLabel}: {financialResult:N2}");
                ctcSb.AppendLine();

                // Accumulate totals
                totalCtc += totalCost;
                totalFinancialResult += financialResult;

                // Syringe-type products are tracked separately
                if (detail.Product?.TypeId == 5)
                    totalSyringeCost += saleAmount;
            }

            // ------------------- Shipping Profit/Loss Calculation -------------------
            ctcSb.AppendLine($"Shipping Method Name: {shippingMethodName}");
            ctcSb.AppendLine($"Shipping Amount: {(order.DeliveryCharge ?? 0m):N2}");
            ctcSb.AppendLine($"Shipping Cost: {(costOfShipping ?? 0m):N2}");
            ctcSb.AppendLine($"IsPriceOverridden: {order.IsDeliveryChargeOverRidden ?? false}");

            var shippingProfit = order.DeliveryCharge - costOfShipping;
            var shippingLabel = shippingProfit >= 0 ? "Profit" : "Loss";
            ctcSb.AppendLine($"{shippingLabel}: {shippingProfit:N2}");
            ctcSb.AppendLine();

            // Overall profit/loss summary
            var summaryTotalSale = order.Subtotal + order.DeliveryCharge;
            var summaryTotalCost = totalCtc + costOfShipping;
            var summaryProfit = summaryTotalSale - summaryTotalCost;

            ctcSb.AppendLine($"Total Sale: {summaryTotalSale:N2}");
            ctcSb.AppendLine($"Total Cost: {summaryTotalCost:N2}");
            string totalProfitLabel = summaryProfit >= 0 ? "Profit" : "Loss";
            ctcSb.AppendLine($"{totalProfitLabel}: {summaryProfit:N2}");

            // Store CTC breakdown into commission object
            commissionsPayable.CtcCalculationDetails = ctcSb.ToString();

            // ------------------- Commission Rate Mapping (Optional Replacement Logic) -------------------
            Dictionary<decimal, decimal>? replacementMap = null;
            if (salesPerson.MatchAsCommisionRate.HasValue &&
                salesPerson.MatchAsCommisionRate == false &&
                !string.IsNullOrWhiteSpace(salesPerson.ReplaceCommisionRate))
            {
                // Parses replacement mapping if salesperson has defined alternate commission rates
                replacementMap = ParseReplacementMap(salesPerson.ReplaceCommisionRate);
            }

            // ------------------- Commission Calculation per Order Detail -------------------
            foreach (var detail in order.OrderDetails)
            {
                sb.AppendLine($"<======================= {detail.Product?.Name} =======================>");
                sb.AppendLine($"Quantity: {detail.Quantity}, Price per unit: {detail.PerUnitAmount}");
                sb.AppendLine($"Product Amount: {detail.Amount}");

                decimal productAmount = (decimal)(detail.Amount ?? 0m);
                decimal ctc = (decimal)(detail.ProductPharmacyPriceListItem?.CostOfProduct ?? 0m) * detail.Quantity;
                decimal financialResult = productAmount - ctc;

                // Syringe-type products: no commission applied
                if (detail.Product?.TypeId == 5)
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

                // Determine applicable commission percentage
                decimal commissionPercentToApply = salesPersonCommissionPercent;

                // If pharmacy has a fixed commission, apply capped rate
                if (pharmacy.HasFixedCommission && pharmacy.CommissionPercentage.HasValue)
                {
                    commissionPercentToApply = Math.Min(pharmacy.CommissionPercentage.Value, salesPersonCommissionPercent);
                    sb.AppendLine($"[FIXED COMMISSION] Applying {commissionPercentToApply:N2}%");
                }
                // Else use dynamic rate from product’s commission range table
                else
                {
                    var applicableRate = detail.Product?.CommisionRates?.FirstOrDefault(cr =>
                        productAmount >= cr.FromAmount &&
                        productAmount <= cr.ToAmount &&
                        cr.IsActive);

                    if (applicableRate != null && applicableRate.RatePercentage.HasValue)
                    {
                        decimal rangeRate = applicableRate.RatePercentage.Value;
                        decimal finalRate = rangeRate;

                        // Apply replacement mapping if defined
                        if (replacementMap != null && replacementMap.TryGetValue(rangeRate, out var replacedRate))
                        {
                            finalRate = replacedRate;
                            sb.AppendLine($"[REPLACEMENT MAP] Original {rangeRate:N2}% replaced with {finalRate:N2}%");
                        }

                        // Ensure salesperson's cap is not exceeded
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

                // Compute actual commission for this order detail
                decimal commissionForDetail = (commissionPercentToApply / 100m) * productAmount;
                totalCommission += commissionForDetail;

                sb.AppendLine($"Calculated commission: {commissionForDetail:N2}");

                // Record commission detail for this product
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

            // ------------------- Final Commission Summary -------------------
            sb.AppendLine($"Commission Base Amount (eligible): {commissionBaseAmount:N2}");
            sb.AppendLine($"Total Commission Applied: {totalCommission:N2}");
            sb.AppendLine($"Syringe Products Total Cost: {totalSyringeCost:N2}");
            sb.AppendLine($"Surcharge Cost: {(order.Surcharge ?? 0m):N2}");
            sb.AppendLine($"Shipping Method Name: {shippingMethodName}");
            sb.AppendLine($"Shipping Cost: {(order.DeliveryCharge ?? 0m):N2}");
            sb.AppendLine($"Total Amount: {order.TotalAmount:N2}");

            // Assign calculated results
            commissionsPayable.CommissionCalculationDetails = sb.ToString();
            commissionsPayable.CommissionPayable = totalCommission;
            commissionsPayable.CTC = totalCtc;
            commissionsPayable.FinancialResult = totalFinancialResult;
            commissionsPayable.SyringeCost = totalSyringeCost;
            commissionsPayable.IsMissingProductPrice = isAnyProductMissingPrice;

            return commissionsPayable;
        }

        /// <summary>
        /// Parses a string representing commission rate replacements into a dictionary.
        /// Example input format: "5:7;10:12" 
        /// → meaning replace 5% with 7% and 10% with 12% when calculating commission.
        /// 
        /// This allows dynamic mapping of original commission rates to new ones,
        /// often used when salespeople have adjusted or capped rates.
        /// </summary>
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

        /// <summary>
        /// Retrieves the list of counselors currently assigned to the active pool
        /// for the current week (Friday to Thursday range).
        /// </summary>
        public async Task<List<PoolDetail>> GetCounselorsInCurrentPoolAsync()
        {
            var (fromDate, toDate) = GetWeekRangeFriThuUtc(DateTime.UtcNow);

            var pool = await _clinicDbContext.Pools
                .FirstOrDefaultAsync(p => p.FromDate == fromDate && p.ToDate == toDate);

            if (pool == null)
                return new List<PoolDetail>();

            var poolDetailsWithCounselors = await _clinicDbContext.PoolDetails
                .Where(pd => pd.PoolId == pool.Id)
                .ToListAsync();

            return poolDetailsWithCounselors;
        }

        /// <summary>
        /// Generates a detailed commission report for a specific counselor.
        /// </summary>
        public async Task<CommissionReportDto> GetCommissionReportByCounselor(int counselorId)
        {
            try
            {
                // Fetch commission-related data for the given counselor.
                //    - Include related Order, Patient, and Counselor details to enrich the report.
                //    - Only include active commission records.
                //    - Project only the required fields into lightweight anonymous objects for performance.
                var commissionData = await _clinicDbContext.CommissionsPayables
                    .Include(cp => cp.Order)
                        .ThenInclude(o => o.Patient)
                    .Include(cp => cp.Order.Counselor)
                    .Where(cp => cp.Order.CounselorId == counselorId && cp.IsActive)
                    .Select(cp => new
                    {
                        CounselorFirstName = cp.Order.Counselor.FirstName,
                        CounselorLastName = cp.Order.Counselor.LastName,
                        EntryType = cp.EntryType,

                        // Build each commission record detail for the report.
                        Detail = new CommissionDetailDto
                        {
                            Patient = $"{cp.Order.Patient.FirstName} {cp.Order.Patient.LastName}",
                            TotalAmount = cp.Order.TotalAmount,
                            Surcharge = cp.Order.Surcharge ?? 0m,
                            Syringe = cp.SyringeCost,
                            Shipping = cp.Order.DeliveryCharge ?? 0m,
                            CommissionAppliedTotalAmount = cp.CommissionBaseAmount,
                            CommissionPayable = cp.CommissionPayable
                        }
                    })
                    .ToListAsync();

                // If no commission data exists for this counselor, return a blank report.
                // This avoids null reference issues and clearly indicates no data found.
                if (!commissionData.Any())
                {
                    return new CommissionReportDto
                    {
                        SalesPersonName = "Unknown",
                        ReportRange = DateTime.Now.ToString("MMMM yyyy"),
                        CommissionDetails = new List<CommissionDetailDto>(),
                        Summary = new CommissionSummaryDto()
                    };
                }

                // Extract basic counselor info from the first record (since all belong to the same counselor).
                var firstRecord = commissionData.First();

                // Separate commission entries into:
                //    - Generated → normal commissions earned.
                //    - Reversal → commissions reversed due to refund/cancellation.
                var generatedItems = commissionData.Where(x => x.EntryType == CommissionEntryTypeEnum.Generated).Select(x => x.Detail).ToList();
                var reversalItems = commissionData.Where(x => x.EntryType == CommissionEntryTypeEnum.Reversal).Select(x => x.Detail).ToList();

                // Calculate net totals by subtracting reversal values from generated ones.
                // This gives the actual payable commission summary for the counselor.
                var netSummary = new CommissionSummaryDto
                {
                    TotalAmount = generatedItems.Sum(d => d.TotalAmount) - reversalItems.Sum(d => d.TotalAmount),
                    TotalSurcharge = generatedItems.Sum(d => d.Surcharge) - reversalItems.Sum(d => d.Surcharge),
                    TotalSyringe = generatedItems.Sum(d => d.Syringe) - reversalItems.Sum(d => d.Syringe),
                    TotalShipping = generatedItems.Sum(d => d.Shipping) - reversalItems.Sum(d => d.Shipping),
                    TotalCommissionAppliedAmount = generatedItems.Sum(d => d.CommissionAppliedTotalAmount) - reversalItems.Sum(d => d.CommissionAppliedTotalAmount),
                    TotalCommissionPayable = generatedItems.Sum(d => d.CommissionPayable) - reversalItems.Sum(d => d.CommissionPayable),
                    RecordCount = generatedItems.Count - reversalItems.Count
                };

                // Combine all commission detail items for display in the report’s table/grid.
                var allDetails = commissionData.Select(x => x.Detail).ToList();

                // Construct and return the final report DTO combining counselor name, details, and summary.
                return new CommissionReportDto
                {
                    SalesPersonName = $"{firstRecord.CounselorFirstName} {firstRecord.CounselorLastName}",
                    ReportRange = DateTime.Now.ToString("MMMM yyyy"),
                    CommissionDetails = allDetails,
                    Summary = netSummary
                };
            }
            catch (Exception)
            {
                // Allow the exception to bubble up — handled at a higher level (e.g., controller or service layer).
                throw;
            }
        }

    }
}
