using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Models
{
    public class RefillInput
    {
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("times_local")]
        public List<string>? TimesLocal { get; set; }

        [JsonPropertyName("weekdays")]
        public string? Weekdays { get; set; }

        [JsonPropertyName("max_occurrences")]
        public int? MaxOccurrences { get; set; } = 200;
    }
}
