using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CommissionsPayableResponseDto
    {
        public Guid PoolId { get; set; }
        public Guid CommissionsPayableId { get; set; }
        public Guid CommissionsPayableDetailId { get; set; }
        public decimal CommissionsPayableAmount { get; set; }
        public string PatientName { get; set; }
        public string PharmacyName { get; set; }
        public decimal TotalSales { get; set; }
        public string? EntryType { get; set; }
        public bool? IsActive { get; set; } 
    }
}
