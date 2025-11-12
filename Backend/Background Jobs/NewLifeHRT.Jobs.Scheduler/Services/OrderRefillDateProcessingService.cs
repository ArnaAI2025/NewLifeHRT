using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using System.Text.Json;
using NewLifeHRT.Infrastructure.Models.RefillCalculation;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public class OrderRefillDateProcessingService : IOrderRefillDateProcessingService
    {
        private readonly ClinicDbContext _clinicDbContext;
        private readonly IAIService _aIService;
        private readonly ILogger<OrderRefillDateProcessingService> _logger;
        private readonly TelemetryClient _telemetryClient;

        public OrderRefillDateProcessingService(
            ClinicDbContext clinicDbContext,
            IAIService aIService,
            ILogger<OrderRefillDateProcessingService> logger,
            TelemetryClient telemetryClient)
        {
            _clinicDbContext = clinicDbContext;
            _aIService = aIService;
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Processes refill date calculations for eligible order details by invoking the AI refill service 
        /// and storing the resulting refill schedules and summaries in the database.
        /// </summary>
        public async Task ProcessRefillDatesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessRefillDatesAsync started at {Time}", DateTime.UtcNow);
            _telemetryClient.TrackTrace("ProcessRefillDatesAsync started.");

            try
            {
                var orderDetails = await _clinicDbContext.OrderDetails
                    .Include(od => od.Order)
                    .Include(od => od.Product)
                    .Where(od => od.IsReadyForRefillDateCalculation == true && od.Order.OrderFulFilled != null)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Found {Count} order details ready for refill calculation", orderDetails.Count);
                _telemetryClient.TrackTrace($"Found {orderDetails.Count} order details ready for refill calculation");

                foreach (var detail in orderDetails)
                {
                    try
                    {
                        var order = detail.Order;
                        var input = new RefillInputModel
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

                        if (result == null)
                        {
                            _logger.LogWarning("AI service returned null for OrderDetailId {OrderDetailId}", detail.Id);
                            _telemetryClient.TrackTrace($"AI service returned null for OrderDetailId {detail.Id}");
                            continue;
                        }

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
                                            _clinicDbContext.OrderProductSchedules.Add(new OrderProductSchedule
                                            {
                                                Id = Guid.NewGuid(),
                                                OrderProductScheduleSummaryId = summaryEntity.Id,
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
                                            _logger.LogWarning("Failed to parse occurrence '{Occurrence}' for OrderDetailId {OrderDetailId}", occ, detail.Id);
                                            _telemetryClient.TrackTrace($"Failed to parse occurrence '{occ}' for OrderDetailId {detail.Id}");
                                        }
                                    }
                                }
                            }
                        }

                        detail.IsReadyForRefillDateCalculation = false;
                        _telemetryClient.TrackTrace($"Processed OrderDetailId {detail.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "AI returned null or invalid JSON for OrderDetailId {OrderDetailId}", detail.Id);
                        _telemetryClient.TrackException(ex);
                    }
                }

                await _clinicDbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("ProcessRefillDatesAsync completed successfully at {Time}", DateTime.UtcNow);
                _telemetryClient.TrackEvent("ProcessRefillDatesAsyncCompleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessRefillDatesAsync");
                _telemetryClient.TrackException(ex);
            }

            _telemetryClient.Flush();
        }

        /// <summary>
        /// Recalculates refill schedules for all processing items marked as "InProgress",
        /// re-evaluating refill dates using the AI refill service and updating corresponding
        /// database entities in a transactional manner.
        /// </summary>
        public async Task RefillRecalculateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RefillRecalculateAsync started at {Time}", DateTime.UtcNow);
            _telemetryClient.TrackTrace("RefillRecalculateAsync started.");

            try
            {
                var processingItems = await _clinicDbContext.ScheduleSummaryProcessings
                    .Where(p => p.Status == "InProgress")
                    .Include(p => p.ScheduleSummary)
                        .ThenInclude(ss => ss.OrderDetail)
                            .ThenInclude(od => od.Product)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Found {Count} schedule items in progress", processingItems.Count);
                _telemetryClient.TrackTrace($"Found {processingItems.Count} schedule items in progress");

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
                            _telemetryClient.TrackTrace($"ScheduleSummary or OrderDetail not found for processing {processing.Id}");
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

                        var input = new RefillInputModel
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

                        if (result == null || string.IsNullOrEmpty(result.Status) || result.Status.Equals("error", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("Invalid refill result for processing {ProcessingId}", processing.Id);
                            _telemetryClient.TrackTrace($"Invalid refill result for processing {processing.Id}");
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
                                        _telemetryClient.TrackTrace($"Failed to parse occurrence_local '{occ}' for summary {summary.Id}");
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
                        _telemetryClient.TrackEvent($"Processed and completed ScheduleSummary {summary.Id}");
                    }
                    catch (Exception innerEx)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(innerEx, "Error processing ScheduleSummaryProcessing {ProcessingId}", processing.Id);
                        _telemetryClient.TrackException(innerEx);

                        var summary = processing.ScheduleSummary;
                        processing.Status = "Error";
                        if (summary != null)
                            summary.Status = "Error";

                        await _clinicDbContext.SaveChangesAsync(cancellationToken);
                    }
                }

                _logger.LogInformation("RefillRecalculateAsync completed successfully at {Time}", DateTime.UtcNow);
                _telemetryClient.TrackEvent("RefillRecalculateAsyncCompleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RefillRecalculateAsync");
                _telemetryClient.TrackException(ex);

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

            _telemetryClient.Flush();
        }
    }
}
