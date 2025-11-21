using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? AddressId { get; set; }
        public virtual Address? Address { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DEA { get; set; }
        public string? NPI { get; set; }
        public decimal? CommisionInPercentage { get; set; }
        public bool? MatchAsCommisionRate { get; set; }
        public string? ReplaceCommisionRate { get; set; }
        public bool Vacation { get; set; }
        public int? TimezoneId { get; set; }
        public string? ColorCode { get; set; }
        public Guid? PatientId { get; set; }
        public bool MustChangePassword { get; set; } = false;
        public virtual Timezone? Timezone { get; set; }
        public virtual Patient Patient { get; set; }

        public virtual ICollection<UserOtp> Otps { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<UserServiceLink> UserServices { get; set; } = new List<UserServiceLink>();
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        public virtual ICollection<Patient> AssignPhysicianPatients { get; set; } = new List<Patient>();
        public virtual ICollection<Patient> CounselorPatients { get; set; } = new List<Patient>();
        public virtual ICollection<Patient> PreviousCounselorPatients { get; set; } = new List<Patient>();
        public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
        public virtual ICollection<CounselorNote> CounselorNotes { get; set; } = new List<CounselorNote>();
        public virtual ICollection<MedicalRecommendation> MedicalRecommendations { get; set; }
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public virtual ICollection<Proposal> PhysicianProposals { get; set; } = new List<Proposal>();
        public virtual ICollection<Proposal> StatusUpdatedBy { get; set; } = new List<Proposal>();
        public virtual ICollection<Order> CounselorOrders { get; set; } = new List<Order>();
        public virtual ICollection<Order> PhysicianOrders { get; set; } = new List<Order>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<BatchMessage> CreatedBatchMessages { get; set; } = new List<BatchMessage>();
        public virtual ICollection<BatchMessage> ApprovedBatchMessages { get; set; } = new List<BatchMessage>();
        public virtual ICollection<PoolDetail> PoolDetails { get; set; } = new List<PoolDetail>();



        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
        public virtual ICollection<LicenseInformation> LicenseInformations { get; set; } = new List<LicenseInformation>();
        public virtual ICollection<UserSignature> UserSignatures { get; set; } = new List<UserSignature>();


        public ApplicationUser() { }

        public ApplicationUser(string userName, string firstName, string lastName, string email, string? phoneNumber, string normalizedEmail,
            string normalizedUserName, string? dea, string? npi, decimal? commisionInPercentage, bool? matchAsCommisionRate, string? replaceCommisionrate,
        bool vacation, int? timezoneId,string? colorCode, Guid? patientId, bool mustChangePassword, string createdBy, DateTime createdAt)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            NormalizedEmail = normalizedEmail;
            NormalizedUserName = normalizedUserName;
            DEA = dea;
            NPI = npi;
            CommisionInPercentage = commisionInPercentage;
            MatchAsCommisionRate = matchAsCommisionRate;
            ReplaceCommisionRate = string.IsNullOrWhiteSpace(replaceCommisionrate) ? null : replaceCommisionrate;
            Vacation = vacation;
            TimezoneId = timezoneId;
            ColorCode = colorCode;
            PatientId = patientId;
            MustChangePassword = mustChangePassword;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }

        public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
        {
            public void Configure(EntityTypeBuilder<ApplicationUser> entity)
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.UserName).IsUnique();

                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(255).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired(false);
                entity.Property(u => u.PasswordHash).IsRequired();

                entity.Property(u => u.DEA).HasMaxLength(100).IsRequired(false);
                entity.Property(u => u.NPI).HasMaxLength(100).IsRequired(false);
                entity.Property(u => u.CommisionInPercentage).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(u => u.MatchAsCommisionRate).IsRequired(false);
                entity.Property(u => u.ReplaceCommisionRate).HasMaxLength(100).IsRequired(false);
                entity.Property(u => u.Vacation).HasDefaultValue(false).IsRequired();
                entity.Property(u => u.ColorCode)
                    .HasMaxLength(10)
                    .IsRequired(false);

                entity.HasOne(u => u.Address)
                      .WithMany()
                      .HasForeignKey(u => u.AddressId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Otps)
                      .WithOne(o => o.User)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.RefreshTokens)
                      .WithOne(r => r.User)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.AssignPhysicianPatients)
                      .WithOne(p => p.AssignPhysician)
                      .HasForeignKey(p => p.AssignPhysicianId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.CounselorPatients)
                      .WithOne(p => p.Counselor)
                      .HasForeignKey(p => p.CounselorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.PreviousCounselorPatients)
                      .WithOne(p => p.PreviousCounselor)
                      .HasForeignKey(p => p.PreviousCounselorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(u => u.IsDeleted).HasDefaultValue(false);
                entity.HasMany(u => u.Leads)
                      .WithOne(l => l.Owner)
                      .HasForeignKey(l => l.OwnerId)
                      .IsRequired(true)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(u => u.CounselorNotes)
                      .WithOne(cn => cn.Counselor)
                      .HasForeignKey(cn => cn.CounselorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.MedicalRecommendations)
                      .WithOne(mr => mr.ApplicationUser)
                      .HasForeignKey(mr => mr.DoctorId);
                entity.HasMany(u => u.Coupons)
                      .WithOne(c => c.User)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(u => u.CounselorOrders)
                      .WithOne(o => o.Counselor)
                      .HasForeignKey(o => o.CounselorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.PhysicianOrders)
                      .WithOne(o => o.Physician)
                      .HasForeignKey(o => o.PhysicianId)
                      .OnDelete(DeleteBehavior.Restrict);


                entity.HasMany(p => p.Appointments)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.DoctorId)
              .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Holidays)
                  .WithOne(h => h.User)
                  .HasForeignKey(h => h.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.Timezone)
                    .WithMany(t => t.Users)
                    .HasForeignKey(u => u.TimezoneId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(u => u.CreatedBatchMessages)
                    .WithOne(bm => bm.MessageCreatedBy)
                    .HasForeignKey(bm => bm.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.ApprovedBatchMessages)
                      .WithOne(bm => bm.ApprovedBy)
                      .HasForeignKey(bm => bm.StatusChangedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(u => u.LicenseInformations)
                  .WithOne(li => li.User)
                  .HasForeignKey(li => li.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(u => u.Patient)
                    .WithOne(p => p.User)
                    .HasForeignKey<ApplicationUser>(u => u.PatientId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

            }

        }
    }
}
