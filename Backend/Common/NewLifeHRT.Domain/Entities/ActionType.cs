using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class ActionType : BaseEntity<int>
    {
        public string Name { get; set; }
        public ActionTypeEnum EnumValue { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public ActionType() { }

        public class ActionTypeConfiguration : IEntityTypeConfiguration<ActionType>
        {
            public void Configure(EntityTypeBuilder<ActionType> entity)
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).HasMaxLength(250).IsRequired(true);
                entity.Property(a => a.EnumValue).HasConversion<string>();
            }
        }
    }
}
