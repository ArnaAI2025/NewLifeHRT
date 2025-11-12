using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System.Diagnostics;

namespace NewLifeHRT.Infrastructure.Data
{
    public class ClinicDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _accessor;
        private readonly ILogger<ClinicDbContext> _logger;
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options, IMultiTenantContextAccessor<MultiTenantInfo> accessor, ILogger<ClinicDbContext> logger) : base(options)
        {
            _accessor = accessor;
            _logger = logger;
        }
        public ClinicDbContext() { }
        public DbSet<UserOtp> UserOtps { get; set; }
        public DbSet<Address> Addresses {  get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<UserServiceLink> UserServiceLinks { get; set; }
        public DbSet<ProductWebForm> ProductWebForms { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<ProductStrength> ProductStrengths { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<PatientAgenda> PatientAgendas { get; set; }
        public DbSet<PatientCreditCard> PatientCreditCards { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<PatientAttachment> PatientAttachments { get; set; }
        public DbSet<VisitType> VisitTypes { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<CounselorNote> CounselorNotes { get; set; }
        public DbSet<MedicalRecommendation> MedicalRecommendations { get; set; }
        public DbSet<MedicationType> MedicationTypes { get; set; }
        public DbSet<FollowUpLabTest> FollowUpLabTests { get; set; }

        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LifeFileScheduleCode> LifeFileScheduleCodes { get; set; }
        public DbSet<LifeFileQuantityUnit> LifeFileQuantityUnits { get; set; }
        public DbSet<LifeFileDrugForm> LifeFileDrugForms { get; set; }
        public DbSet<ProductPharmacyPriceListItem> ProductPharmacyPriceListItems { get; set; }
        public DbSet<CommisionRate> CommisionRates { get; set; }
        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<PharmacyShippingMethod> PharmacyShippingMethods { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ProposalDetail> ProposalDetails { get; set; }
        public DbSet<AppointmentMode> AppointmentModes { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HolidayDate> HolidayDates { get; set; }
        public DbSet<HolidayRecurrence> HolidayRecurrences { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PharmacyOrderTracking> PharmacyOrderTrackings { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<BatchMessage> BatchMessages { get; set; }
        public DbSet<BatchMessageRecipient> BatchMessageRecipients { get; set; }
        public DbSet<Conversation> Conversations{ get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageContent> MessagesContent { get; set; }
        public DbSet<ReminderType> ReminderTypes { get; set; }
        public DbSet<RecurrenceRule> RecurrenceRules { get; set; }
        public DbSet<PatientReminder> PatientReminders { get; set; }
        public DbSet<LeadReminder> LeadReminders { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<IntegrationType> IntegrationTypes { get; set; }
        public DbSet<IntegrationKey> IntegrationKeys { get; set; }
        public DbSet<PharmacyConfigurationEntity> PharmacyConfigurations { get; set; } 
        public DbSet<PharmacyConfigurationData> PharmacyConfigurationDatas { get; set; }

        public DbSet<CommissionsPayable> CommissionsPayables {  get; set; }
        public DbSet<CommissionsPayablesDetail> CommissionsPayablesDetails { get; set; }
        public DbSet<Pool> Pools { get; set; }
        public DbSet<PoolDetail> PoolDetails { get; set; }

        public DbSet<Timezone> TimeZones { get; set; }
        public DbSet<OrderProcessingApiTracking> OrderProcessingApiTrackings { get; set; }
        public DbSet<OrderProcessingApiTransaction> OrderProcessingApiTransactions { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<LicenseInformation> LicenseInformation { get; set; }
        public DbSet<OrderProductRefillDetail> OrderProductRefillDetails { get; set; }
        public DbSet<OrderProductSchedule> OrderProductSchedules { get; set; }
        public DbSet<OrderProductScheduleSummary> OrderProductScheduleSummaries { get; set; }
        public DbSet<ScheduleSummaryProcessing> ScheduleSummaryProcessings { get; set; }
        public DbSet<PatientSelfReminder> PatientSelfReminders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ApplicationRole.RoleConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUser.ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new Permission.PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermission.RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new Address.AddressConfiguration());
            modelBuilder.ApplyConfiguration(new UserOtp.UserOtpConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshToken.RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new Service.ServiceConfiguration());
            modelBuilder.ApplyConfiguration(new UserServiceLink.UserServiceLinkConfiguration());
            modelBuilder.ApplyConfiguration(new ProductWebForm.ProductWebFormConfiguration());
            modelBuilder.ApplyConfiguration(new ProductType.ProductTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategory.ProductCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductStatus.ProductStatusConfiguration());
            modelBuilder.ApplyConfiguration(new ProductStrength.ProductStrengthConfiguration());
            modelBuilder.ApplyConfiguration(new Product.ProductConfiguration());
            modelBuilder.ApplyConfiguration(new Agenda.AgendaConfiguration());
            modelBuilder.ApplyConfiguration(new PatientAgenda.PatientAgendaConfiguration());
            modelBuilder.ApplyConfiguration(new PatientCreditCard.PatientCreditCardConfiguration());
            modelBuilder.ApplyConfiguration(new Attachment.AttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new PatientAttachment.PatientAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new VisitType.VisitTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Patient.PatientConfiguration());
            modelBuilder.ApplyConfiguration(new Pharmacy.PharmacyConfiguration());
            modelBuilder.ApplyConfiguration(new Currency.CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new Lead.LeadConfiguration());
            modelBuilder.ApplyConfiguration(new LifeFileScheduleCode.LifeFileScheduleCodeConfiguration());
            modelBuilder.ApplyConfiguration(new LifeFileQuantityUnit.LifeFileQuantityUnitConfiguration());
            modelBuilder.ApplyConfiguration(new LifeFileDrugForm.LifeFileDrugFormConfiguration());
            modelBuilder.ApplyConfiguration(new ProductPharmacyPriceListItem.ProductPharmacyPriceListItemConfiguration());
            modelBuilder.ApplyConfiguration(new CounselorNote.CounselorNoteConfiguration());  
            modelBuilder.ApplyConfiguration(new MedicalRecommendation.MedicalRecommendationConfiguration());  
            modelBuilder.ApplyConfiguration(new MedicationType.MedicationTypeConfiguration());  
            modelBuilder.ApplyConfiguration(new FollowUpLabTest.FollowUpLabTestConfiguration());  
            modelBuilder.ApplyConfiguration(new CommisionRate.CommisionRatePriceListItemConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentCategory.DocumentCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ShippingAddress.ShippingAddressConfiguration());
            modelBuilder.ApplyConfiguration(new Proposal.ProposalConfiguration());
            modelBuilder.ApplyConfiguration(new ProposalDetail.ProposalDetailConfiguration());
            modelBuilder.ApplyConfiguration(new Coupon.CouponConfiguration());
            modelBuilder.ApplyConfiguration(new ShippingMethod.ShippingMethodConfiguration());
            modelBuilder.ApplyConfiguration(new PharmacyShippingMethod.PharmacyShippingMethodConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentMode.AppointmentModeConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentStatus.AppointmentStatusConfiguration());
            modelBuilder.ApplyConfiguration(new Slot.SlotConfiguration());
            modelBuilder.ApplyConfiguration(new Appointment.AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new Holiday.HolidayConfiguration());
            modelBuilder.ApplyConfiguration(new HolidayDate.HolidayDateConfiguration());
            modelBuilder.ApplyConfiguration(new HolidayRecurrence.HolidayRecurrenceConfiguration());
            modelBuilder.ApplyConfiguration(new Order.OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetail.OrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new PharmacyOrderTracking.PharmacyOrderTrackingConfiguration());
            modelBuilder.ApplyConfiguration(new Timezone.TimezoneConfiguration());
            modelBuilder.ApplyConfiguration(new Country.CountryConfiguration());
            modelBuilder.ApplyConfiguration(new CommissionsPayable.CommissionsPayableConfiguration());
            modelBuilder.ApplyConfiguration(new CommissionsPayablesDetail.CommissionsPayablesDetailConfiguration());
            modelBuilder.ApplyConfiguration(new Pool.PoolConfiguration());
            modelBuilder.ApplyConfiguration(new PoolDetail.PoolDetailConfiguration());
            modelBuilder.ApplyConfiguration(new BatchMessage.BatchMessageConfiguration());
            modelBuilder.ApplyConfiguration(new BatchMessageRecipient.BatchMessageRecipientConfiguration());
            modelBuilder.ApplyConfiguration(new Conversation.ConversationConfiguration());
            modelBuilder.ApplyConfiguration(new Message.MessageConfiguration());
            modelBuilder.ApplyConfiguration(new MessageContent.MessageContentConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationType.IntegrationTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationKey.IntegrationKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PharmacyConfigurationEntity.PharmacyConfigurationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PharmacyConfigurationData.PharmacyConfigurationDataConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProcessingApiTracking.OrderProcessingApiTrackingConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProcessingApiTransaction.OrderProcessingApiTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new State.StateConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseInformation.LicenseInformationConfiguration());
            modelBuilder.ApplyConfiguration(new ReminderType.ReminderTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Reminder.ReminderConfiguration());
            modelBuilder.ApplyConfiguration(new RecurrenceRule.RecurrenceRuleConfiguration());
            modelBuilder.ApplyConfiguration(new PatientReminder.PatientReminderConfiguration());
            modelBuilder.ApplyConfiguration(new LeadReminder.LeadReminderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductSchedule.OrderProductScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleSummaryProcessing.ScheduleSummaryProcessingConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var tenantInfo = _accessor.MultiTenantContext.TenantInfo;
            Debug.Assert(tenantInfo != null, "No tenant info found.");

            if (string.IsNullOrEmpty(tenantInfo?.ConnectionString))
            {
                throw new InvalidOperationException("No tenant connection string found."); 
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Setting connection string for tenant {TenantName}", tenantInfo.Name);
            }
            optionsBuilder.UseSqlServer(tenantInfo.ConnectionString);
        }
    }
}
