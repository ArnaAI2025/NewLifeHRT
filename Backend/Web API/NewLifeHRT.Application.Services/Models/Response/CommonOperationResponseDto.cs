using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CommonOperationResponseDto<TId>
    {
        public TId Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
