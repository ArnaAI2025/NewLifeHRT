using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class CommissionsPayablesDetail : BaseEntity<Guid>
    {
        public Guid CommissionsPayableId { get; set; }
        public Guid OrderDetailId { get; set; }
        public decimal CTC { get; set; }
        public decimal FinancialResult { get; set; }
        public string CommissionType { get; set; }
        public string CommissionPercentage { get; set; }
        //public decimal Amount { get; set; }
        public decimal CommissionPayable { get; set; }
        public virtual CommissionsPayable CommissionsPayable { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        public class CommissionsPayablesDetailConfiguration : IEntityTypeConfiguration<CommissionsPayablesDetail>
        {
            public void Configure(EntityTypeBuilder<CommissionsPayablesDetail> entity)
            {
                entity.HasKey(cpd => new { cpd.CommissionsPayableId, cpd.OrderDetailId });

                entity.Property(cpd => cpd.CTC)
                      .HasColumnType("decimal(18,2)");

                entity.Property(cpd => cpd.FinancialResult)
                      .HasColumnType("decimal(18,2)");

                //entity.Property(cpd => cpd.TotalAmount)
                //      .HasColumnType("decimal(18,2)");

                //entity.Property(cpd => cpd.Amount)
                //      .HasColumnType("decimal(18,2)")
                //      .IsRequired();

                entity.Property(cpd => cpd.CommissionPayable)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(cpd => cpd.CommissionsPayable)
                 .WithMany(cp => cp.CommissionsPayablesDetails)
                 .HasForeignKey(cpd => cpd.CommissionsPayableId)
                 .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cpd => cpd.OrderDetail)
                      .WithMany(od => od.CommissionsPayablesDetails)
                      .HasForeignKey(cpd => cpd.OrderDetailId)
                      .OnDelete(DeleteBehavior.Restrict);
            }

        }
    }
}
