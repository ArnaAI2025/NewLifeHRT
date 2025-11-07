using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class RecurrenceRuleMappings
    {
        public static RecurrenceRuleResponseDto ToRecurrenceRuleResponseDto(this RecurrenceRule recurrenceRule)
        {
            return new RecurrenceRuleResponseDto
            {
                Id = recurrenceRule.Id,
                RuleName = recurrenceRule.RuleName,
            };
        }

        public static List<RecurrenceRuleResponseDto> ToRecurrenceRuleResponseDtoList(this IEnumerable<RecurrenceRule> recurrenceRule)
        {
            return recurrenceRule.Select(recurrenceRule => recurrenceRule.ToRecurrenceRuleResponseDto()).ToList();
        }
    }
}
