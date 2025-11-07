using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Attachment : BaseEntity<Guid>
    {
        public string AttachmentName { get; set; }
        public string FileName { get; set; }
        public string? FileType { get; set; }
        public string Extension { get; set; }
        public string? Source { get; set; }
        public int DocumentCategoryId { get; set; }
        public virtual DocumentCategory DocumentCategory { get; set; }
        public virtual ICollection<PatientAttachment> PatientAttachments { get; set; }
        public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
        {
            public void Configure(EntityTypeBuilder<Attachment> builder)
            {

                builder.HasKey(a => a.Id);

                builder.Property(a => a.AttachmentName)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(a => a.FileName)
                       .IsRequired()
                       .HasMaxLength(255);

                builder.Property(a => a.FileType)
                       .HasMaxLength(100);

                builder.Property(a => a.Extension)
                       .IsRequired()
                       .HasMaxLength(10);

                builder.Property(a => a.Source)
                       .HasMaxLength(255);

                builder.HasMany(a => a.PatientAttachments)
                       .WithOne(pa => pa.Attachment)
                       .HasForeignKey(pa => pa.AttachmentId)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(a => a.DocumentCategory)
                        .WithMany(dc => dc.Attachments)
                        .HasForeignKey(a => a.DocumentCategoryId)
                        .OnDelete(DeleteBehavior.Restrict);

            }
        }
    }
}
