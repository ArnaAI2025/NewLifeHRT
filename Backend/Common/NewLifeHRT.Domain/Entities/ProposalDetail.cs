using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ProposalDetail : BaseEntity<Guid>
    {
        public Guid ProductPharmacyPriceListItemId { get; set; }
        public Guid ProposalId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal? Amount { get; set; }
        public decimal PerUnitAmount { get; set; }
        public bool? IsPriceOverRidden { get; set; }
        public string? Protocol { get; set; }

        public virtual Proposal Proposal { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductPharmacyPriceListItem ProductPharmacyPriceListItem { get; set; }

        public class ProposalDetailConfiguration : IEntityTypeConfiguration<ProposalDetail>
        {
            public void Configure(EntityTypeBuilder<ProposalDetail> builder)
            {
                builder.HasKey(pd => pd.Id);

                builder.Property(pd => pd.Quantity)
                       .IsRequired();

                builder.Property(pd => pd.Amount)
                       .HasColumnType("decimal(18,2)");

                builder.Property(pd => pd.PerUnitAmount)
                       .HasColumnType("decimal(18,2)");

                builder.HasOne(pd => pd.Proposal)
                       .WithMany(p => p.ProposalDetails)
                       .HasForeignKey(pd => pd.ProposalId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(pd => pd.Product)
                        .WithMany(p => p.ProposalDetails)   
                        .HasForeignKey(pd => pd.ProductId)
                        .OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(pd => pd.ProductPharmacyPriceListItem)
                        .WithMany(p => p.ProposalDetails)
                        .HasForeignKey(pd => pd.ProductPharmacyPriceListItemId)
                        .OnDelete(DeleteBehavior.Restrict);

            }
        }
    }

}
