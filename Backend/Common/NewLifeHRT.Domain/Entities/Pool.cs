using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;

public class Pool : BaseEntity<Guid>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int Week { get; set; }
    public virtual ICollection<PoolDetail> PoolDetails { get; set; } = new List<PoolDetail>();

    public class PoolConfiguration : IEntityTypeConfiguration<Pool>
    {
        public void Configure(EntityTypeBuilder<Pool> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FromDate)
                .IsRequired();
            builder.Property(p => p.ToDate)
                .IsRequired();
            builder.Property(p => p.Week)
                .IsRequired();

            builder.HasMany(p => p.PoolDetails)
                .WithOne(pd => pd.Pool)
                .HasForeignKey(pd => pd.PoolId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}