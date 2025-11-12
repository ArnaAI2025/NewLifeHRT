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
    public class WellsGetTokenModel
    {
        public class Request
        {
            public string ApiKey { get; set; }
        }
        public class Response
        {
            public string Token { get; set; }

        }
    }

}
