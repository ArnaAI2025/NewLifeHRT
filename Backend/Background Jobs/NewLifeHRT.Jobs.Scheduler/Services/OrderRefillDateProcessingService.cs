using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Common.Helpers;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Models;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Models.Encryption;
using NewLifeHRT.Jobs.Scheduler.Interface;
using NewLifeHRT.Jobs.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public class OrderRefillDateProcessingService : IOrderRefillDateProcessingService
    {
        private readonly ClinicDbContext _clinicDbContext;
        private readonly IAIService _aIService;
        private readonly ILogger<OrderRefillDateProcessingService> _logger;
        public OrderRefillDateProcessingService(ClinicDbContext clinicDbContext, IAIService aIService, ILogger<OrderRefillDateProcessingService> logger)
        {
            _clinicDbContext = clinicDbContext;
            _aIService = aIService;
            _logger = logger;
        }

        public async Task ProcessRefillDatesAsync(CancellationToken cancellationToken)
        {
            try
            {


                var orderDetails = await _clinicDbContext.OrderDetails
                    .Include(od => od.Order)
                    .Include(od => od.Product)
                    .Where(od => od.IsReadyForRefillDateCalculation == true &&
                         od.Order.OrderFulFilled != null)
                    .ToListAsync(cancellationToken);

                foreach (var detail in orderDetails)
                {
                    var order = detail.Order;
                    var input = new RefillInput
                    {
                        ProductName = detail.Product?.Name ?? string.Empty,
                        Protocol = detail.Protocol ?? string.Empty,
                        Quantity = detail.Quantity,
                        StartDate = DateOnly.FromDateTime(order.OrderFulFilled.Value.AddDays(2)),
                        Timezone = "EST",
                        TimesLocal = ["08:00", "20:00"],
                        MaxOccurrences = 300
                    };

                    var result = await _aIService.CalculateNextRefillAsync(input, cancellationToken);

                    if (result != null)
                    {
                        var refillEntity = new OrderProductRefillDetail
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            ProductPharmacyPriceListItemId = detail.ProductPharmacyPriceListItemId,
                            Status = result.Status,
                            DaysSupply = result.Days_Supply,
                            RefillDate = DateOnly.TryParse(result.Refill_Date, out var rd) ? rd : null,
                            DoseAmount = (decimal?)result.Details?.Dose_Amount,
                            DoseUnit = result.Details?.Dose_Unit,
                            FrequencyPerDay = result.Details?.Frequency_Per_Day,
                            FrequencyPerWeek = result.Details?.Frequency_Per_Week,
                            BottleSizeMl = (decimal?)result.Details?.Bottle_Size_Ml,
                            VialCount = result.Details?.Vial_Count,
                            UnitsCount = result.Details?.Units_Count,
                            ClicksPerBottle = result.Details?.Clicks_Per_Bottle,
                            Assumptions = result.Details?.Assumptions ?? new List<string>(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };

                        _clinicDbContext.OrderProductRefillDetails.Add(refillEntity);

                        if (result.Schedule != null)
                        {
                            var daysCsv = result.Schedule.Weekdays != null && result.Schedule.Weekdays.Any()
                                ? string.Join(",", result.Schedule.Weekdays)
                                : null;
                            var summaryEntity = new OrderProductScheduleSummary
                            {
                                Id = Guid.NewGuid(),
                                OrderDetailId = detail.Id,
                                FrequencyType = result.Schedule.Pattern,
                                StartDate = input.StartDate,
                                EndDate = refillEntity.RefillDate,
                                Days = daysCsv,
                                CreatedAt = DateTime.UtcNow,
                                Status = "Completed",
                                IsActive = true
                            };

                            _clinicDbContext.OrderProductScheduleSummaries.Add(summaryEntity);

                            if (result.Schedule.OccurrencesLocal != null)
                            {
                                foreach (var occSeq in result.Schedule.OccurrencesLocal)
                                {
                                    if (occSeq?.OccurrenceLocal == null) continue;

                                    foreach (var occ in occSeq.OccurrenceLocal)
                                    {
                                        if (DateTime.TryParse(occ, out var occurrenceDateTime))
                                        {
                                            var scheduleEntity = new OrderProductSchedule
                                            {
                                                Id = Guid.NewGuid(),
                                                OrderProductScheduleSummaryId = summaryEntity.Id,
                                                TimeZone = result.Schedule.Timezone,
                                                OccurrenceDateAndTime = occurrenceDateTime,
                                                Sequence = occSeq.Sequence,
                                                Truncated = result.Schedule.Truncated,
                                                CreatedAt = DateTime.UtcNow,
                                                IsActive = true
                                            };

                                            _clinicDbContext.OrderProductSchedules.Add(scheduleEntity);
                                        }
                                        else
                                        {
                                            _logger.LogWarning("Failed to parse occurrence_local '{Occurrence}' for OrderDetailId {OrderDetailId}", occ, detail.Id);
                                        }
                                    }

                                }
                            }
                        }

                        detail.IsReadyForRefillDateCalculation = false;
                    }

                    else
                    {
                        _logger.LogWarning("AI returned null or invalid JSON for OrderDetailId {OrderDetailId}. Response: {AIResponse}",
                            detail.Id, result);
                    }
                }
                await _clinicDbContext.SaveChangesAsync(cancellationToken);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _logger.LogError(ex, "Error in ProcessRefillDatesAsync");
            }
        }

        public async Task RefillRecalculateAsync(CancellationToken cancellationToken)
        {
            try
            {
                var processingItems = await _clinicDbContext.ScheduleSummaryProcessings
                    .Where(p => p.Status == "InProgress")
                    .Include(p => p.ScheduleSummary)
                        .ThenInclude(ss => ss.OrderDetail)
                            .ThenInclude(od => od.Product)
                    .ToListAsync(cancellationToken);

                foreach (var processing in processingItems)
                {
                    await using var transaction = await _clinicDbContext.Database.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        var summary = processing.ScheduleSummary;
                        var orderDetail = summary?.OrderDetail;

                        if (summary == null || orderDetail == null)
                        {
                            _logger.LogWarning("ScheduleSummary or OrderDetail not found for processing {ProcessingId}", processing.Id);
                            processing.Status = "Error";
                            if (summary != null) summary.Status = "Error";
                            await _clinicDbContext.SaveChangesAsync(cancellationToken);
                            await transaction.CommitAsync(cancellationToken);
                            continue;
                        }

                        var timesLocal = !string.IsNullOrWhiteSpace(processing.TimesLocal)
                            ? (processing.TimesLocal.TrimStart().StartsWith("[")
                                ? JsonSerializer.Deserialize<List<string>>(processing.TimesLocal) ?? new List<string>()
                                : processing.TimesLocal.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(t => t.Trim())
                                    .ToList())
                            : new List<string>();

                        var input = new RefillInput
                        {
                            ProductName = orderDetail.Product?.Name ?? string.Empty,
                            Protocol = orderDetail.Protocol ?? string.Empty,
                            Quantity = orderDetail.Quantity,
                            StartDate = processing.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                            Timezone = "EST",
                            TimesLocal = timesLocal,
                            Weekdays = processing.Days,
                            MaxOccurrences = 300
                        };

                        var result = await _aIService.CalculateNextRefillAsync(input, cancellationToken);

                        if (result == null ||
                            string.IsNullOrEmpty(result.Status) ||
                            result.Status.Equals("error", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("Invalid refill result for processing {ProcessingId}", processing.Id);
                            processing.Status = "Error";
                            summary.Status = "Error";
                            await _clinicDbContext.SaveChangesAsync(cancellationToken);
                            await transaction.CommitAsync(cancellationToken);
                            continue;
                        }

                        var existingSchedules = await _clinicDbContext.OrderProductSchedules
                            .Where(x => x.OrderProductScheduleSummaryId == summary.Id)
                            .ToListAsync(cancellationToken);

                        if (existingSchedules.Any())
                        {
                            _clinicDbContext.OrderProductSchedules.RemoveRange(existingSchedules);
                        }

                        if (result.Schedule?.OccurrencesLocal != null)
                        {
                            foreach (var occSeq in result.Schedule.OccurrencesLocal)
                            {
                                if (occSeq?.OccurrenceLocal == null) continue;

                                foreach (var occ in occSeq.OccurrenceLocal)
                                {
                                    if (DateTime.TryParse(occ, out var occurrenceDateTime))
                                    {
                                        _clinicDbContext.OrderProductSchedules.Add(new OrderProductSchedule
                                        {
                                            Id = Guid.NewGuid(),
                                            OrderProductScheduleSummaryId = summary.Id,
                                            TimeZone = result.Schedule.Timezone,
                                            OccurrenceDateAndTime = occurrenceDateTime,
                                            Sequence = occSeq.Sequence,
                                            Truncated = result.Schedule.Truncated,
                                            CreatedAt = DateTime.UtcNow,
                                            IsActive = true
                                        });
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Failed to parse occurrence_local '{Occurrence}' for summary {SummaryId}", occ, summary.Id);
                                    }
                                }
                            }
                        }

                        summary.StartDate = processing.StartDate;
                        summary.Days = processing.Days;
                        summary.TimesLocal = processing.TimesLocal;

                        if (DateOnly.TryParse(result.Refill_Date, out var refillDate))
                            summary.EndDate = refillDate;

                        summary.Status = "Completed";
                        summary.UpdatedAt = DateTime.UtcNow;

                        processing.Status = "Completed";
                        _clinicDbContext.ScheduleSummaryProcessings.Remove(processing);

                        await _clinicDbContext.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);

                        _logger.LogInformation("Processed and completed ScheduleSummary {SummaryId}", summary.Id);
                    }
                    catch (Exception innerEx)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(innerEx, "Error processing ScheduleSummaryProcessing {ProcessingId}", processing.Id);

                        var summary = processing.ScheduleSummary;
                        processing.Status = "Error";
                        if (summary != null)
                            summary.Status = "Error";

                        await _clinicDbContext.SaveChangesAsync(cancellationToken);
                    }
                }

                _logger.LogInformation("RefillRecalculateAsync completed successfully at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RefillRecalculateAsync");

                var inProgress = await _clinicDbContext.ScheduleSummaryProcessings
                    .Where(p => p.Status == "InProgress")
                    .Include(p => p.ScheduleSummary)
                    .ToListAsync(cancellationToken);

                foreach (var item in inProgress)
                {
                    item.Status = "Error";
                    if (item.ScheduleSummary != null)
                        item.ScheduleSummary.Status = "Error";
                }

                await _clinicDbContext.SaveChangesAsync(cancellationToken);
                throw;
            }
        }
    }
}
