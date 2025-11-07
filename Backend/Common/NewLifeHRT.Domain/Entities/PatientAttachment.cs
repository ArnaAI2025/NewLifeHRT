using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class PatientAttachment : BaseEntity<Guid>
    {
        public Guid PatientId { get; set; }
        public Guid AttachmentId { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Attachment Attachment { get; set; }


        public class PatientAttachmentConfiguration : IEntityTypeConfiguration<PatientAttachment>
        {
            public void Configure(EntityTypeBuilder<PatientAttachment> builder)
            {

                builder.HasKey(pa => pa.Id);

                builder.Property(pa => pa.PatientId)
                       .IsRequired();

                builder.Property(pa => pa.AttachmentId)
                       .IsRequired();

                builder.HasOne(pa => pa.Patient)
                       .WithMany(p => p.PatientAttachments)
                       .HasForeignKey(pa => pa.PatientId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(pa => pa.Attachment)
                       .WithMany(a => a.PatientAttachments)
                       .HasForeignKey(pa => pa.AttachmentId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
