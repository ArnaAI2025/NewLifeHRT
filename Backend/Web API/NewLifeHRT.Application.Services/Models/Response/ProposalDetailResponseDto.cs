using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ProposalDetailResponseDto
    {
        public Guid? Id { get; set; }
        public Guid ProductPharmacyPriceListItemId { get; set; }
        public Guid? ProposalId { get; set; }
        public string? ProductName { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal PerUnitAmount { get; set; }
        public bool? IsColdStorageProduct { get; set; }
        public string Protocol { get; set; }


    }
}
