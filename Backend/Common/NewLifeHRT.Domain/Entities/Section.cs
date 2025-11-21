using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class Section : BaseEntity<int>
    {
        public string Name { get; set; }
        public string EnumValue { get; set; }
        public string? ModuleName { get; set; }
        public SectionLevelEnum Level { get; set; }

        public int? ParentId {  get; set; }
        public Section Parent { get; set; }
        public ICollection<Section> Childrem{ get; set; } = new List<Section>();
        public virtual ICollection<Permission> Permissions { get; set; }

        public Section() { }

        public class SectionConfiguration : IEntityTypeConfiguration<Section>
        {
            public void Configure(EntityTypeBuilder<Section> entity)
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).HasMaxLength(250).IsRequired(true);
                entity.Property(a => a.EnumValue).HasMaxLength(250).IsRequired(true);
                entity.Property(r => r.Level).HasConversion<string>();
                entity.HasOne(a => a.Parent)
                      .WithMany(c => c.Childrem)
                      .HasForeignKey(a => a.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
