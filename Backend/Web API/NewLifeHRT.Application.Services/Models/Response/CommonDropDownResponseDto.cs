using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CommonDropDownResponseDto<TId>
    {
        public TId Id { get; set; }
        public string Value { get; set; }
    }

}
