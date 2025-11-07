using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class OrderProductScheduleSummaryMappings
    {
        public static OrderProductScheduleSummaryDetailResponseDto ToDetailDto(this OrderProductScheduleSummary entity)
        {
            return new OrderProductScheduleSummaryDetailResponseDto
            {
                OrderProductScheduleSummaryId = entity.Id,
                ProductName = entity.OrderDetail?.Product?.Name ?? string.Empty,
                Protocol = entity.OrderDetail?.Protocol ?? string.Empty,
                FrequencyType = entity.FrequencyType ?? string.Empty,
                StartDate = entity.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                Days = entity.Days ?? string.Empty,
                Times = entity.OrderProductSchedules?
                            .GroupBy(x => x.Sequence)
                            .Select(g =>
                                g.OrderBy(x => x.OccurrenceDateAndTime)
                                 .FirstOrDefault()?.OccurrenceDateAndTime?.ToString("hh:mm tt"))
                            .Where(t => !string.IsNullOrEmpty(t))
                            .ToList() ?? new List<string>()
            };
        }
    }
}
