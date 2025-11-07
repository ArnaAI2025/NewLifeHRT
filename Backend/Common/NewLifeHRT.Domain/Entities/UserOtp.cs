using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace NewLifeHRT.Domain.Entities
{
    public class UserOtp : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public string OtpCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public string SentToEmail { get; set; }

        public virtual ApplicationUser User { get; set; }

        public UserOtp() { }
        public UserOtp(int userId, string otpCode, DateTime createdAt, DateTime expiresAt, bool isUsed, string sentToEmail)
        {
            UserId = userId;
            OtpCode = otpCode;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            IsUsed = isUsed;
            SentToEmail = sentToEmail;
        }

        public class UserOtpConfiguration : IEntityTypeConfiguration<UserOtp>
        {
            public void Configure(EntityTypeBuilder<UserOtp> entity)
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OtpCode).HasMaxLength(6).IsRequired();
                entity.Property(o => o.SentToEmail).HasMaxLength(255).IsRequired();

                entity.HasOne(o => o.User)
                       .WithMany(u => u.Otps)
                       .HasForeignKey(o => o.UserId);
            }
        }
    }
}
