using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class DocumentCategory : BaseEntity<int>
    {
        public string CategoryName { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public class DocumentCategoryConfiguration : IEntityTypeConfiguration<DocumentCategory>
        {
            public void Configure(EntityTypeBuilder<DocumentCategory> builder)
            {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.CategoryName).IsRequired().HasMaxLength(100);
                builder.Property(p => p.CategoryName)
                   .IsRequired()
                   .HasMaxLength(100);
            }
        }
    }
}
