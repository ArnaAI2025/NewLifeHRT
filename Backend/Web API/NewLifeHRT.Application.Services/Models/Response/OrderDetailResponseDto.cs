using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderDetailResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductPharmacyPriceListItemId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public bool? IsColdStorageProduct { get; set; }
        public bool? IsActive { get; set; }
        public decimal PerUnitAmount { get; set; }
        public decimal? Amount { get; set; }
        public string Protocol { get; set; }
        public string ProductType { get; set; }

    }
}
