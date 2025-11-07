using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Jobs.Scheduler.Interface;
using System.Globalization;
using System.Text;
using CommissionReportDto = NewLifeHRT.Jobs.Scheduler.Models.CommissionReportDto;
using CommissionDetailDto = NewLifeHRT.Jobs.Scheduler.Models.CommissionDetailDto;
using CommissionSummaryDto = NewLifeHRT.Jobs.Scheduler.Models.CommissionSummaryDto;
using NewLifeHRT.Domain.Enums;

public class WeeklyCommissionService : IWeeklyCommissionService
{
    private readonly ClinicDbContext _clinicDbContext;

    public WeeklyCommissionService(ClinicDbContext clinicDbContext)
    {
        _clinicDbContext = clinicDbContext;
    }

    private static (DateTime From, DateTime To) GetWeekRangeFriThuUtc(DateTime todayUtc)
    {
        var today = todayUtc.Date;
        int diffToFriday = ((int)today.DayOfWeek - (int)DayOfWeek.Friday + 7) % 7;
        var from = today.AddDays(-diffToFriday);
        var to = from.AddDays(6);
        return (from, to);
    }

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

    public async Task<(Pool? Pool, List<PoolDetail> PoolDetails)> GetTodayPoolAndDetailsAsync()
    {
        var (from, to) = GetWeekRangeFriThuUtc(DateTime.UtcNow);
        var pool = await EnsurePoolExistsAsync(from, to);
        var details = await _clinicDbContext.PoolDetails
            .Where(pd => pd.PoolId == pool.Id)
            .ToListAsync();

        return (pool, details);
    }

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

    public static CommissionsPayable? CalculateCommission(Order order)
    {
        if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
            return null;

        var sb = new StringBuilder();
        var ctcSb = new StringBuilder();

        var pharmacy = order.Pharmacy;
        var salesPerson = order.Counselor;

        if (pharmacy == null || salesPerson == null)
            return null;

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
        var costOfShipping = order.PharmacyShippingMethod?.CostOfShipping;
        decimal salesPersonCommissionPercent = salesPerson.CommisionInPercentage ?? 0m;

        // Track missing price
        bool isAnyProductMissingPrice = false;

        // CTC and financials build
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

            if (detail.Product?.TypeId == 5)
                totalSyringeCost += saleAmount;
        }

        ctcSb.AppendLine($"Shipping Method Name: {shippingMethodName}");
        ctcSb.AppendLine($"Shipping Amount: {(order.DeliveryCharge ?? 0m):N2}");
        ctcSb.AppendLine($"Shipping Cost: {(costOfShipping ?? 0m):N2}");
        ctcSb.AppendLine($"IsPriceOverridden: {order.IsDeliveryChargeOverRidden ?? false}");
        var shippingProfit = order.DeliveryCharge - costOfShipping;
        var shippingLabel = shippingProfit >= 0 ? "Profit" : "Loss";
        ctcSb.AppendLine($"{shippingLabel}: {shippingProfit:N2}");
        ctcSb.AppendLine();

        var summaryTotalSale = order.Subtotal + order.DeliveryCharge;
        var summaryTotalCost = totalCtc + costOfShipping;
        var summaryProfit = summaryTotalSale - summaryTotalCost;
        ctcSb.AppendLine($"Total Sale: {(summaryTotalSale):N2}");
        ctcSb.AppendLine($"Total Cost: {summaryTotalCost:N2}");
        string totalProfitLabel = summaryProfit >= 0 ? "Profit" : "Loss";
        ctcSb.AppendLine($"{totalProfitLabel}: {summaryProfit:N2}");

        commissionsPayable.CtcCalculationDetails = ctcSb.ToString();

        // Replacement map for salesperson caps/rate changes (optional)
        Dictionary<decimal, decimal>? replacementMap = null;
        if (salesPerson.MatchAsCommisionRate.HasValue &&
            salesPerson.MatchAsCommisionRate == false &&
            !string.IsNullOrWhiteSpace(salesPerson.ReplaceCommisionRate))
        {
            replacementMap = ParseReplacementMap(salesPerson.ReplaceCommisionRate);
        }

        // Line-level commission computation
        foreach (var detail in order.OrderDetails)
        {
            sb.AppendLine($"<======================= {detail.Product?.Name} =======================>");
            sb.AppendLine($"Quantity: {detail.Quantity}, Price per unit: {detail.PerUnitAmount}");
            sb.AppendLine($"Product Amount: {detail.Amount}");

            decimal productAmount = (decimal)(detail.Amount ?? 0m);
            decimal ctc = (decimal)(detail.ProductPharmacyPriceListItem?.CostOfProduct ?? 0m) * detail.Quantity;
            decimal financialResult = productAmount - ctc;

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

            decimal commissionPercentToApply = salesPersonCommissionPercent;
            if (pharmacy.HasFixedCommission && pharmacy.CommissionPercentage.HasValue)
            {
                commissionPercentToApply = Math.Min(pharmacy.CommissionPercentage.Value, salesPersonCommissionPercent);
                sb.AppendLine($"[FIXED COMMISSION] Applying {commissionPercentToApply:N2}%");
            }
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
                    if (replacementMap != null && replacementMap.TryGetValue(rangeRate, out var replacedRate))
                    {
                        finalRate = replacedRate;
                        sb.AppendLine($"[REPLACEMENT MAP] Original {rangeRate:N2}% replaced with {finalRate:N2}%");
                    }
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

    public async Task<CommissionReportDto> GetCommissionReportByCounselor(int counselorId)
    {
        try
        {
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

            var firstRecord = commissionData.First();

            var generatedItems = commissionData.Where(x => x.EntryType == CommissionEntryTypeEnum.Generated).Select(x => x.Detail).ToList();
            var reversalItems = commissionData.Where(x => x.EntryType == CommissionEntryTypeEnum.Reversal).Select(x => x.Detail).ToList();

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

            var allDetails = commissionData.Select(x => x.Detail).ToList();

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
            throw;
        }
    }


}
