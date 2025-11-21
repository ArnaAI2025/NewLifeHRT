using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class UserSignature : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public string? SignaturePath { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public UserSignature(){}

        public UserSignature(int userId, string? signaturePath)
        {
            UserId = userId;
            SignaturePath = signaturePath;
        }

        public class UserSignatureConfiguration : IEntityTypeConfiguration<UserSignature>
        {
            public void Configure(EntityTypeBuilder<UserSignature> entity)
            {
                entity.HasKey(us => us.Id);

                entity.Property(us => us.SignaturePath);

                entity.HasOne(us => us.User)
                      .WithMany(u => u.UserSignatures)
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
