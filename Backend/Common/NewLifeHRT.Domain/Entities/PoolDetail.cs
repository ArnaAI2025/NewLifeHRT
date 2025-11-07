using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;

public class PoolDetail : BaseEntity<Guid>
{
    public Guid PoolId { get; set; }
    public int CounselorId { get; set; }
    public virtual Pool Pool { get; set; }
    public virtual ApplicationUser Counselor { get; set; }

    public virtual ICollection<CommissionsPayable> CommissionsPayables { get; set; } = new List<CommissionsPayable>();

    public class PoolDetailConfiguration : IEntityTypeConfiguration<PoolDetail>
    {
        public void Configure(EntityTypeBuilder<PoolDetail> builder)
        {
            builder.HasKey(pd => pd.Id);

            builder.Property(pd => pd.CounselorId)
                .IsRequired();

            builder.HasOne(pd => pd.Pool)
                .WithMany(p => p.PoolDetails)
                .HasForeignKey(pd => pd.PoolId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pd => pd.Counselor)
                   .WithMany(u => u.PoolDetails)  
                   .HasForeignKey(pd => pd.CounselorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}