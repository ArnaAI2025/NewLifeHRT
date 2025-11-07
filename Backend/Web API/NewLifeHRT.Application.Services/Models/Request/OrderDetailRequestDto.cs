using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class OrderDetailRequestDto
    {
        public Guid? OrderId { get; set; }
        public Guid? Id { get; set; }
        public Guid? OrderDetailId { get; set; }
        public Guid ProductPharmacyPriceListItemId { get; set; }
        public bool? IsPriceOverRidden { get; set; }

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Protocol { get; set; }
        public decimal PerUnitAmount { get; set; }
        public decimal? Amount { get; set; }
    }
}
