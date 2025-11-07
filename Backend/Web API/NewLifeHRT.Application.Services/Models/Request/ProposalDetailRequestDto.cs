using System;

namespace NewLifeHRT.Domain.DTOs
{
    public class ProposalDetailRequestDto
    {
        public Guid? Id { get; set; }
        public Guid ProductPharmacyPriceListItemId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal PerUnitAmount { get; set; }
        public string? Protocol { get; set; }


    }
}
