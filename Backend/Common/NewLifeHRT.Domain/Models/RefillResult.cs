using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Models
{
    public class RefillResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("days_supply")]
        public int Days_Supply { get; set; }

        [JsonPropertyName("refill_date")]
        public string? Refill_Date { get; set; }

        [JsonPropertyName("details")]
        public OrderProductRefillDetailDetailsDto? Details { get; set; }

        [JsonPropertyName("schedule")]
        public RefillSchedule? Schedule { get; set; }
    }

    public class OrderProductRefillDetailDetailsDto
    {
        [JsonPropertyName("dose_amount")]
        public decimal? Dose_Amount { get; set; }

        [JsonPropertyName("dose_unit")]
        public string? Dose_Unit { get; set; }

        [JsonPropertyName("frequency_per_day")]
        public decimal? Frequency_Per_Day { get; set; }

        [JsonPropertyName("frequency_per_week")]
        public decimal? Frequency_Per_Week { get; set; }

        [JsonPropertyName("bottle_size_ml")]
        public decimal? Bottle_Size_Ml { get; set; }

        [JsonPropertyName("vial_count")]
        public decimal? Vial_Count { get; set; }

        [JsonPropertyName("units_count")]
        public decimal? Units_Count { get; set; }

        [JsonPropertyName("clicks_per_bottle")]
        public decimal? Clicks_Per_Bottle { get; set; }

        [JsonPropertyName("assumptions")]
        public List<string>? Assumptions { get; set; }
    }

    public class RefillSchedule
    {
        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("pattern")]
        public string? Pattern { get; set; }

        [JsonPropertyName("weekdays")]
        public List<string>? Weekdays { get; set; }

        [JsonPropertyName("occurrences_local")]
        public List<OccurrenceSequence>? OccurrencesLocal { get; set; }

        [JsonPropertyName("truncated")]
        public bool? Truncated { get; set; }
    }

    public class OccurrenceSequence
    {
        [JsonPropertyName("sequence")]
        public int Sequence { get; set; }

        [JsonPropertyName("occurrence_local")]
        public List<string>? OccurrenceLocal { get; set; }
    }
}
