using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class CommissionsPayable : BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public Guid PoolDetailId { get; set; }
        //public decimal? TotalAmount { get; set; }
        public decimal CommissionBaseAmount { get; set; }
        public decimal SyringeCost { get; set; }
        public decimal CommissionPayable { get; set; }
        public decimal CTC { get; set; }
        public decimal FinancialResult { get; set; }
        public string? CommissionCalculationDetails { get; set; }
        public string? CtcCalculationDetails { get; set; }
        public bool? IsMissingProductPrice { get; set; }
        public CommissionEntryTypeEnum? EntryType { get; set; } = CommissionEntryTypeEnum.Generated;
        public virtual Order Order { get; set; }

        public virtual PoolDetail PoolDetail { get; set; }
        public virtual ICollection<CommissionsPayablesDetail> CommissionsPayablesDetails { get; set; } = new List<CommissionsPayablesDetail>();
        public class CommissionsPayableConfiguration : IEntityTypeConfiguration<CommissionsPayable>
        {
            public void Configure(EntityTypeBuilder<CommissionsPayable> entity)
            {
                entity.HasKey(cp => cp.Id);

                //entity.Property(cp => cp.TotalAmount)
                //      .HasColumnType("decimal(18,2)");

                entity.Property(cp => cp.CommissionBaseAmount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(cp => cp.SyringeCost)
                      .HasColumnType("decimal(18,2)");

                entity.Property(cp => cp.CommissionPayable)
                      .HasColumnType("decimal(18,2)");

                entity.Property(cp => cp.CTC)
                      .HasColumnType("decimal(18,2)");

                entity.Property(cp => cp.FinancialResult)
                      .HasColumnType("decimal(18,2)");

                entity.Property(cp => cp.CommissionCalculationDetails)
                      .HasMaxLength(2000);

                entity.Property(cp => cp.CtcCalculationDetails)
                      .HasMaxLength(2000);

                //entity.HasOne<Order>()
                //      .WithMany(o => o.CommissionsPayables) 
                //      .HasForeignKey(cp => cp.OrderId)
                //      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cp => cp.PoolDetail)
                      .WithMany(pd => pd.CommissionsPayables)
                      .HasForeignKey(cp => cp.PoolDetailId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(p => p.EntryType).IsRequired(false).HasMaxLength(10).HasConversion<string>();

                entity.HasIndex(e => new { e.OrderId, e.PoolDetailId, e.EntryType }).IsUnique();
                entity.HasOne(cp => cp.Order)
                     .WithMany(o => o.CommissionsPayables)
                     .HasForeignKey(cp => cp.OrderId)
                     .OnDelete(DeleteBehavior.Restrict);



            }
        }
    }
}
