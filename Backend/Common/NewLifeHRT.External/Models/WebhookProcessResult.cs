using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class WebhookProcessResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? HttpStatusCode { get; set; }
    }
}

