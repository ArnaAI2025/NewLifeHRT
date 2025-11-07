using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PriceListItemGetByIdResponseDto
    {
        public Guid Id { get; set; }
        public int? CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public decimal? CostOfProduct { get; set; }
        public string? LifeFilePharmacyProductId { get; set; }
        public string? LifeFielForeignPmsId { get; set; }
        public int? LifeFileDrugFormId { get; set; }
        public string? LifeFileDrugName { get; set; }
        public string? LifeFileDrugStrength { get; set; }
        public int? LifeFileQuantityUnitId { get; set; }
        public int? LifeFileScheduleCodeId { get; set; }
        public Guid PharmacyId { get; set; }
        public Guid ProductId { get; set; }
        public string Status { get; set; }
    }
}
