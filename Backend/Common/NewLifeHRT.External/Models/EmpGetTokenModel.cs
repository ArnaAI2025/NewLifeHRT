using NewLifeHRT.External.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class EmpGetTokenModel
    {
        public class Request
        {
            public string BaseUrl { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
        }
        public class Response
        {

            [JsonPropertyName("token")]
            public string Token { get; set; }

            [JsonPropertyName("tokenCreatedTime")]
            public DateTime TokenCreatedTime { get; set; }

            [JsonPropertyName("tokenExpirationTime")]
            public DateTime TokenExpirationTime { get; set; }
        }
    }

}
