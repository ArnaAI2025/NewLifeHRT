using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace NewLifeHRT.Domain.Entities
{
    public class RefreshToken : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string CreatedByIp { get; set; }
        public string UserAgent { get; set; }

        public virtual ApplicationUser User { get; set; }

        public RefreshToken() { }
        public RefreshToken(string userAgent, int userId, string token, DateTime expiresAt, string createdByIP, DateTime createdAt) : base(createdAt)
        {
            UserAgent = userAgent;
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedByIp = createdByIP;
        }

        public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
        {
            public void Configure(EntityTypeBuilder<RefreshToken> entity)
            {
                entity.HasKey(x => x.Id);
                entity.Property(r => r.Token).HasMaxLength(512).IsRequired();
                entity.Property(r => r.CreatedByIp).HasMaxLength(100);
                entity.Property(r => r.UserAgent).HasMaxLength(255);

                entity.HasOne(r => r.User)
                       .WithMany(u => u.RefreshTokens)
                       .HasForeignKey(r => r.UserId);
            }
        }
    }
}
