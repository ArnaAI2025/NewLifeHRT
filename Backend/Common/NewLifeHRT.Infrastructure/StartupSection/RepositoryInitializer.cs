using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class RepositoryInitializer
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserOtpRepository, UserOtpRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IUserServiceLinkRepository, UserServiceLinkRepository>();
            services.AddScoped<IClinicServiceRepository, ClinicServiceRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IProductStatusRepository, ProductStatusRepository>();
            services.AddScoped<IProductWebFormRepository, ProductWebFormRepository>();
            services.AddScoped<IProductStrengthRepository, ProductStrengthRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IVisitTypeRepository, VisitTypeRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IPatientAttachmentRepository, PatientAttachmentRepository>();
            services.AddScoped<IAgendaRepository, AgendaRepository>();
            services.AddScoped<IPharmacyRepository, PharmacyRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<IPriceListItemRepository, PriceListItemRepository>();
            services.AddScoped<ILifeFileDrugFormRepository, LifeFileDrugFormRepository>();
            services.AddScoped<ILifeFileQuantityUnitsRepository, LifeFileQuantityUnitsRepository>();
            services.AddScoped<ILifeFileScheduleCodeRepository, LifeFileScheduleCodeRepository>();
            services.AddScoped<ICounselorNoteRepository, CounselorNoteRepository>();
            services.AddScoped<IMedicalRecommendationRepository, MedicalRecommendationRepository>();
            services.AddScoped<IMedicationTypeRepository, MedicationTypeRepository>();
            services.AddScoped<IFollowUpLabTestRepository, FollowUpLabTestRepository>();
            services.AddScoped<ICommisionRateRepository, CommisionRateRepository>();
            services.AddScoped<IPatientAgendaRepository, PatientAgendaRepository>();
            services.AddScoped<IPatientCreditCardRepository, PatientCreditCardRepository>();
            services.AddScoped<IDocumentCategoryRepository, DocumentCategoryRepository>();
            services.AddScoped<IShippingAddressRepository, ShippingAddressRepository>();
            services.AddScoped<IProposalRepository, ProposalRepository>();
            services.AddScoped<IProposalDetailRepository, ProposalDetailRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<IShippingMethodRepository, ShippingMethodRepository>();
            services.AddScoped<IPharmacyShippingMethodRepository, PharmacyShippingMethodRepository>();
            services.AddScoped<ISlotRepository, SlotRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IAppointmentModeRepository, AppointmentModeRepository>();
            services.AddScoped<IHolidayRepository, HolidayRepository>();
            services.AddScoped<IHolidayDateRepository, HolidayDateRepository>();
            services.AddScoped<IHolidayRecurrenceRepository, HolidayRecurrenceRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IPharmacyOrderTrackingRepository, PharmacyOrderTrackingRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ITimezoneRepository, TimezoneRepository>();
            services.AddScoped<IBatchMessageRepository, BatchMessageRepository>();
            services.AddScoped<IBatchMessageRecipientRepository, BatchMessageRecipientRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageContentRepository, MessageContentRepository>();
            services.AddScoped<IPharmacyConfigurationRepository, PharmacyConfigurationRepository>();
            services.AddScoped<IIntegrationTypeRepository, IntegrationTypeRepository>();
            services.AddScoped<IIntegrationKeyRepository, IntegrationKeyRepository>();
            services.AddScoped<IPharmacyConfigurationDataRepository, PharmacyConfigurationDataRepository>();
            services.AddScoped<IOrderProcessingApiTrackingRepository, OrderProcessingApiTrackingRepository>();
            services.AddScoped<IPoolRepository, PoolRepository>();
            services.AddScoped<IPoolDetailRepository, PoolDetailRepository>();
            services.AddScoped<ICommissionsPayableRepository, CommissionsPayableRepository>();
            services.AddScoped<ICommissionsPayableDetailRepository, CommissionsPayableDetailRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ILicenseInformationRepository, LicenseInformationRepository>();
            services.AddScoped<IReminderRepository, ReminderRepository>();
            services.AddScoped<IReminderTypeRepository, ReminderTypeRepository>();
            services.AddScoped<IRecurrenceRuleRepository, RecurrenceRuleRepository>();
            services.AddScoped<IPatientReminderRepository, PatientReminderRepository>();
            services.AddScoped<ILeadReminderRepository, LeadReminderRepository>();
            services.AddScoped<IOrderProductsRefillRepository, OrderProductsRefillRepository>();
            services.AddScoped<IOrderProductScheduleRepository, OrderProductScheduleRepository>();
            services.AddScoped<IOrderProductScheduleSummaryRepository, OrderProductScheduleSummaryRepository>();
            services.AddScoped<IScheduleSummaryProcessingRepository, ScheduleSummaryProcessingRepository>();
            services.AddScoped<IPatientSelfReminderRepository, PatientSelfReminderRepository>();
            return services;
        }
    }
}
