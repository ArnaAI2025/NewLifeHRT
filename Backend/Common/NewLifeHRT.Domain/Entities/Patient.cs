using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class Patient : BaseEntity<Guid>
    {
        public int? VisitTypeId { get; set; }
        public bool? SplitCommission { get; set; }
        public string? PatientGoal { get; set; }
        public string? PatientNumber { get; set; }
        public string? WellsPatientId { get; set; }
        public Guid? ReferralId { get; set; }
        public virtual Patient Referral { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? DrivingLicence { get; set; }

        public Guid? AddressId { get; set; }
        public virtual Address? Address { get; set; }

        public int? AssignPhysicianId { get; set; }
        public int CounselorId { get; set; }
        public int? PreviousCounselorId { get; set; }

        public string? Allergies { get; set; }
        public bool Status { get; set; }
        public bool? IsAllowMail { get; set; }
        public DateTime? LabRenewableAlertDate { get; set; }
        public decimal OutstandingRefundBalance { get; set; }

        public virtual ApplicationUser AssignPhysician { get; set; }
        public virtual ApplicationUser Counselor { get; set; }
        public virtual ApplicationUser PreviousCounselor { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual VisitType VisitType { get; set; }

        public ICollection<PatientCreditCard> PatientCreditCards { get; set; } = new List<PatientCreditCard>();
        public ICollection<PatientAgenda> PatientAgendas { get; set; } = new List<PatientAgenda>();
        public ICollection<PatientAttachment> PatientAttachments { get; set; } = new List<PatientAttachment>();
        public virtual ICollection<Patient> ReferredPatients { get; set; } = new List<Patient>();
        public virtual ICollection<CounselorNote> CounselorNotes { get; set; }
        public virtual ICollection<MedicalRecommendation> MedicalRecommendations { get; set; }
        public ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Conversation> Conversations { get; set; }
        public virtual ICollection<BatchMessageRecipient> BatchMessageRecipients { get; set; } = new List<BatchMessageRecipient>();
        public virtual ICollection<PatientReminder> PatientReminders { get; set; } = new List<PatientReminder>();

        public virtual ICollection<PatientSelfReminder> PatientSelfReminders { get; set; } = new List<PatientSelfReminder>();


        public Patient(
            int? visitTypeId, bool? splitCommission, string? patientGoal, string? patientNumber,
            Guid? referralId, string firstName, string lastName, GenderEnum? gender, string? phoneNumber,
            string? email, DateTime? dateOfBirth, string? drivingLicence, Guid? addressId,
            int? assignPhysicianId, int counselorId, int? previousCounselorId,
            string allergies, bool status, bool? isAllowMail, DateTime? labRenewableAlertDate,
            decimal outstandingRefundBalance,
            bool isActive, string createdBy, DateTime createdAt)
        {
            VisitTypeId = visitTypeId;
            SplitCommission = splitCommission;
            PatientGoal = patientGoal;
            PatientNumber = patientNumber;
            ReferralId = referralId;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Email = email;
            DateOfBirth = dateOfBirth;
            DrivingLicence = drivingLicence;
            AddressId = addressId;
            AssignPhysicianId = assignPhysicianId;
            CounselorId = counselorId;
            PreviousCounselorId = previousCounselorId;
            Allergies = allergies;
            Status = status;
            IsAllowMail = isAllowMail;
            LabRenewableAlertDate = labRenewableAlertDate;
            OutstandingRefundBalance = outstandingRefundBalance;
            IsActive = isActive;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }
        public void UpdatePatient(
            int? visitTypeId,
            bool? splitCommission,
            string patientGoal,
            Guid? referralId,
            string firstName,
            string lastName,
            GenderEnum? gender,
            string phoneNumber,
            string? email,
            DateTime? dateOfBirth,
            string drivingLicence,
            Guid? addressId,
            int? assignPhysicianId,
            int counselorId,
            int? previousCounselorId,
            string allergies,
            bool status,
            bool? isAllowMail,
            DateTime? labRenewableAlertDate,
            decimal outstandingRefundBalance,
            string updatedBy,
            DateTime updatedAt)
        {
            VisitTypeId = visitTypeId;
            SplitCommission = splitCommission;
            PatientGoal = patientGoal;
            ReferralId = referralId;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Email = email;
            DateOfBirth = dateOfBirth;
            DrivingLicence = drivingLicence;
            AddressId = addressId;
            AssignPhysicianId = assignPhysicianId;
            CounselorId = counselorId;
            PreviousCounselorId = previousCounselorId;
            Allergies = allergies;
            Status = status;
            IsAllowMail = isAllowMail;
            LabRenewableAlertDate = labRenewableAlertDate;
            OutstandingRefundBalance = outstandingRefundBalance;
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }
        public class PatientConfiguration : IEntityTypeConfiguration<Patient>
        {
            public void Configure(EntityTypeBuilder<Patient> builder)
            {
                builder.HasKey(p => p.Id);

                builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                builder.Property(p => p.Email).IsRequired(false).HasMaxLength(255);
                builder.Property(p => p.Gender).IsRequired(false).HasMaxLength(10).HasConversion<string>();
                builder.Property(p => p.PhoneNumber).HasMaxLength(20);
                builder.Property(p => p.DrivingLicence).HasMaxLength(50);
                builder.Property(p => p.Allergies).IsRequired(false).HasMaxLength(255);
                builder.Property(p => p.PatientGoal).HasMaxLength(500);
                builder.Property(p => p.PatientNumber).IsRequired(false).HasMaxLength(50);
                builder.Property(p => p.OutstandingRefundBalance)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                builder.Property(p => p.WellsPatientId)
                       .IsRequired(false)
                       .HasMaxLength(100);

                builder.HasOne(p => p.Referral)
                       .WithMany(p => p.ReferredPatients)
                       .HasForeignKey(p => p.ReferralId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.VisitType)
                       .WithMany(v => v.Patients)
                       .HasForeignKey(p => p.VisitTypeId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.Address)
                       .WithMany()
                       .HasForeignKey(p => p.AddressId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.AssignPhysician)
                       .WithMany(u => u.AssignPhysicianPatients)
                       .HasForeignKey(p => p.AssignPhysicianId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.Counselor)
                       .WithMany(u => u.CounselorPatients)
                       .HasForeignKey(p => p.CounselorId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.PreviousCounselor)
                       .WithMany(u => u.PreviousCounselorPatients)
                       .HasForeignKey(p => p.PreviousCounselorId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(p => p.PatientCreditCards)
                       .WithOne(cc => cc.Patient)
                       .HasForeignKey(cc => cc.PatientId);

                builder.HasMany(p => p.PatientAgendas)
                       .WithOne(pa => pa.Patient)
                       .HasForeignKey(pa => pa.PatientId);

                builder.HasMany(p => p.PatientAttachments)
                       .WithOne(pa => pa.Patient)
                       .HasForeignKey(pa => pa.PatientId);
                builder.HasMany(p => p.CounselorNotes)
                       .WithOne(cn => cn.Patient)
                       .HasForeignKey(cn => cn.PatientId)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(p => p.MedicalRecommendations)
                       .WithOne(mr => mr.Patient)
                       .HasForeignKey(mr => mr.PatientId);
                builder.HasMany(p => p.ShippingAddresses)
                        .WithOne(sa => sa.Patient)
                        .HasForeignKey(sa => sa.PatientId)
                        .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(p => p.Appointments)
                  .WithOne(a => a.Patient)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);

            }
        }
    }
}
