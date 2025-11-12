using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CommissionsPayableDetailResponseDto
    {
        public string CounselorName { get; set; }
        public string? CommissionPayStatus { get; set; }
        public string? PatientName  { get; set; }
        public string? WeekSummary  { get; set; }
        public string? OrdersName { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? Shipping { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? Syringe { get; set; }
        public decimal? CommissionAppliedTotal { get; set; }
        public decimal? CommissionPayable { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public string? PharmacyName { get; set; }
        public string? CommissionCalculationDetails { get; set; }
        public string? CtcCalculationDetails { get; set; }
        public decimal? Ctc { get; set; }
        public decimal? CtcPlusCommission { get; set; }
        public decimal? ProfitAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public bool? IsPriceOverRidden { get; set; }
        public Guid? PoolDetailId { get; set; }
        public bool? IsMissingProductPrice { get; set; }


    }
}
