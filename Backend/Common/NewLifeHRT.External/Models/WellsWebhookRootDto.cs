using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class WellsWebhookRootDto
    {
        [JsonProperty("events")]
        public List<WellsWebhookDto> Events { get; set; }
    }
    public class WellsWebhookDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_on")]
        public string CreatedOn { get; set; }

        [JsonProperty("shard_code")]
        public string ShardCode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public WellsEventDataDto Data { get; set; }
    }

    public class WellsEventDataDto
    {
        [JsonProperty("external_order_id")]
        public string ExternalOrderId { get; set; }

        [JsonProperty("pharmacy_rx_nbr")]
        public string PharmacyRxNbr { get; set; }

        [JsonProperty("wpn_order_nbr")]
        public string WpnOrderNbr { get; set; }

        [JsonProperty("tracking_nbr")]
        public string TrackingNbr { get; set; }

        [JsonProperty("carrier")]
        public string Carrier { get; set; }
    }

}
