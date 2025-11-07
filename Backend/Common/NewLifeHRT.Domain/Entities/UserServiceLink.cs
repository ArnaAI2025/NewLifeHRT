using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class UserServiceLink : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public Guid ServiceId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Service Service { get; set; }
        public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

        public UserServiceLink() { }

        public UserServiceLink(int userId, Guid serviceId, string createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            UserId = userId;
            ServiceId = serviceId;
        }

        public class UserServiceLinkConfiguration : IEntityTypeConfiguration<UserServiceLink>
        {
            public void Configure(EntityTypeBuilder<UserServiceLink> entity)
            {
                entity.HasKey(us => us.Id);

                entity.Property(us => us.UserId)
                       .IsRequired();

                entity.Property(us => us.ServiceId)
                      .IsRequired();

                entity.HasOne(us => us.User)
                      .WithMany(u => u.UserServices)
                      .HasForeignKey(us => us.UserId);

                entity.HasOne(us => us.Service)
                      .WithMany(u => u.UserServices)
                      .HasForeignKey(us => us.ServiceId);
            }
        }
    }
}
