using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class BulkOperationAssigneeRequestDto<TIds, TId>
        : BulkOperationRequestDto<TIds>
    {
        public TId? Id { get; set; }
    }
}
