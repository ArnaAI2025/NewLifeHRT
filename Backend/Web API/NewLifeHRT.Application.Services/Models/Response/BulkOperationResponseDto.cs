using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class BulkOperationResponseDto
    {
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> FailedIds { get; set; } = new List<string>();
        public List<string> SuccessIds { get; set; } = new List<string>();
    }
}
