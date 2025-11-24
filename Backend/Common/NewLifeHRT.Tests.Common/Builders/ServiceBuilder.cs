using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Common.Interfaces;
using NewLifeHRT.Application.Services.Services.Hubs;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.External.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Tests.Common.Builders
{
    public abstract class ServiceBuilder<T> : BaseServiceBuilder<T> where T : class
    {
        protected Mock<IUserRepository> UserRepositoryMock { get; set; } = new();
        protected Mock<IAddressRepository> AddressRepositoryMock { get; set; } = new();
        protected Mock<IAgendaRepository> AgendaRepositoryMock { get; set; } = new();
        protected Mock<IAppointmentModeRepository> AppointmentModeRepositoryMock { get; set; } = new();
        protected Mock<IAppointmentRepository> AppointmentRepositoryMock { get; set; } = new();
        protected Mock<IHolidayRepository> HolidayRepositoryMock { get; set; } = new();
        protected Mock<IHolidayDateRepository> HolidayDateRepositoryMock { get; set; } = new();
        protected Mock<IHolidayRecurrenceRepository> HolidayRecurrenceRepositoryMock { get; set; } = new();
        protected Mock<IIntegrationTypeRepository> IntegrationTypeRepositoryMock { get; set; } = new();
        protected Mock<IIntegrationKeyRepository> IntegrationKeyRepositoryMock { get; set; } = new();
        protected Mock<IPharmacyConfigurationRepository> PharmacyConfigurationRepositoryMock { get; set; } = new();
        protected Mock<IPharmacyConfigurationDataRepository> PharmacyConfigurationDataRepositoryMock { get; set; } = new();
        protected Mock<IPharmacyRepository> PharmacyRepositoryMock { get; set; } = new();
        protected Mock<IPharmacyShippingMethodRepository> PharmacyShippingMethodRepositoryMock { get; set; } = new();
        protected Mock<ISlotRepository> SlotRepositoryMock { get; set; } = new();
        protected Mock<ITimezoneRepository> TimezoneRepositoryMock { get; set; } = new();
        protected Mock<IPatientRepository> PatientRepositoryMock { get; set; } = new();
        protected Mock<IShippingAddressRepository> ShippingAddressRepositoryMock { get; set; } = new();
        protected Mock<IPatientAttachmentRepository> PatientAttachmentRepositoryMock { get; set; } = new();
        protected Mock<IAttachmentRepository> AttachmentRepositoryMock { get; set; } = new();
        protected Mock<IPasswordHasher<ApplicationUser>> PasswordHasherMock { get; set; } = new();
        protected Mock<UserManager<ApplicationUser>> UserManagerMock { get; set; } = new(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
        protected Mock<IUserServiceLinkRepository> UserServiceLinkRepositoryMock { get; set; } = new();
        protected Mock<ClinicDbContext> ClinicDbContextMock { get; set; } = new();
        protected Mock<ILicenseInformationService> LicenseInformationServiceMock { get; set; } = new();
        protected Mock<IBlobService> BlobServiceMock { get; set; } = new();
        protected Mock<AzureBlobStorageSettings> AzureBlobStorageSettingsMock { get; set; } = new();
        protected IOptions<AzureBlobStorageSettings> AzureBlobStorageOptions { get; set; } = Options.Create(new AzureBlobStorageSettings());
        protected Mock<IUserSignatureRepository> UserSignatureRepositoryMock { get; set; } = new();
        protected Mock<RoleManager<ApplicationRole>> RoleManagerMock { get; set; } = new();
        protected Mock<IUserOtpRepository> UserOtpRepositoryMock { get; set; } = new();
        protected Mock<IRefreshTokenRepository> RefreshTokenRepositoryMock { get; set; } = new();
        protected Mock<IJwtService> JwtServiceMock { get; set; } = new();
        protected Mock<IMessageRepository> MessageRepositoryMock { get; set; } = new();
        protected Mock<IPatientAgendaService> PatientAgendaServiceMock { get; set; } = new();
        protected Mock<IPatientCreditCardService> PatientCreditCardServiceMock { get; set; } = new();
        protected Mock<IAddressService> AddressServiceMock { get; set; } = new();
        protected Mock<IPatientAttachmentService> PatientAttachmentServiceMock { get; set; } = new();
        protected Mock<IAttachmentService> AttachmentServiceMock { get; set; } = new();
        protected Mock<IShippingAddressService> ShippingAddressServiceMock { get; set; } = new();
        protected Mock<IMessageContentService> MessageContentServiceMock { get; set; } = new();
        protected Mock<ISingletonService> SingletonServiceMock { get; set; } = new();
        protected Mock<IPharmacyShippingMethodService> PharmacyShippingMethodServiceMock { get; set; } = new();
        protected Mock<IUserService> UserServiceMock { get; set; } = new();
        protected Mock<IProductRepository> ProductRepositoryMock { get; set; } = new();
        protected Mock<IProductStrengthRepository> ProductStrengthRepositoryMock { get; set; } = new();
        protected Mock<IPriceListItemRepository> PriceListItemRepositoryMock { get; set; } = new();
        protected Mock<IProductStatusRepository> ProductStatusRepositoryMock { get; set; } = new();
        protected Mock<IProductTypeRepository> ProductTypeRepositoryMock { get; set; } = new();
        protected Mock<IProductCategoryRepository> ProductCategoryRepositoryMock { get; set; } = new();
        protected Mock<IProductWebFormRepository> ProductWebFormRepositoryMock { get; set; } = new();
        protected Mock<IBatchMessageRepository> BatchMessageRepositoryMock { get; set; } = new();
        protected Mock<IBatchMessageRecipientRepository> BatchMessageRecipientRepositoryMock { get; set; } = new();
        protected Mock<IConversationRepository> ConversationRepositoryMock { get; set; } = new();
        protected Mock<IMessageContentRepository> MessageContentRepositoryMock { get; set; } = new();
        protected Mock<ISmsService> SmsServiceMock { get; set; } = new();
        protected Mock<IPatientService> PatientServiceMock { get; set; } = new();
        protected Mock<ILeadService> LeadServiceMock { get; set; } = new();
        protected Mock<IMessageService> MessageServiceMock { get; set; } = new();
        protected Mock<IHubContext<SmsHub>> HubContextMock { get; set; } = new();
        protected Mock<IAudioConverter> AudioConverterMock { get; set; } = new();
        protected Mock<IReminderRepository> ReminderRepositoryMock { get; set; } = new();
        protected Mock<IReminderTypeRepository> ReminderTypeRepositoryMock { get; set; } = new();
        protected Mock<IRecurrenceRuleRepository> RecurrenceRuleRepositoryMock { get; set; } = new();
        protected Mock<ILeadReminderRepository> LeadReminderRepositoryMock { get; set; } = new();
        protected Mock<IPatientReminderRepository> PatientReminderRepositoryMock { get; set; } = new();
        protected Mock<ILeadRepository> LeadRepositoryMock { get; set; } = new();
        protected Mock<IBatchMessageRecipientService> BatchMessageRecipientServiceMock { get; set; } = new();

        public ServiceBuilder<T> SetParameter(Mock<IUserRepository> userRepositoryMock)
        {
            UserRepositoryMock = userRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IAddressRepository> addressRepositoryMock)
        {
            AddressRepositoryMock = addressRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IAgendaRepository> agendaRepositoryMock)
        {
            AgendaRepositoryMock = agendaRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IAppointmentModeRepository> appointmentModeRepositoryMock)
        {
            AppointmentModeRepositoryMock = appointmentModeRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IAppointmentRepository> appointmentRepositoryMock)
        {
            AppointmentRepositoryMock = appointmentRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IHolidayRepository> holidayRepositoryMock)
        {
            HolidayRepositoryMock = holidayRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IHolidayDateRepository> holidayDateRepositoryMock)
        {
            HolidayDateRepositoryMock = holidayDateRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IHolidayRecurrenceRepository> holidayRecurrenceRepositoryMock)
        {
            HolidayRecurrenceRepositoryMock = holidayRecurrenceRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IIntegrationTypeRepository> integrationTypeRepositoryMock)
        {
            IntegrationTypeRepositoryMock = integrationTypeRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IIntegrationKeyRepository> integrationKeyRepositoryMock)
        {
            IntegrationKeyRepositoryMock = integrationKeyRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IPharmacyRepository> pharmacyRepositoryMock)
        {
            PharmacyRepositoryMock = pharmacyRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IPharmacyConfigurationRepository> pharmacyConfigurationRepositoryMock)
        {
            PharmacyConfigurationRepositoryMock = pharmacyConfigurationRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IPharmacyConfigurationDataRepository> pharmacyConfigurationDataRepositoryMock)
        {
            PharmacyConfigurationDataRepositoryMock = pharmacyConfigurationDataRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<ISlotRepository> slotRepositoryMock)
        {
            SlotRepositoryMock = slotRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<ITimezoneRepository> timezoneRepositoryMock)
        {
            TimezoneRepositoryMock = timezoneRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IPharmacyShippingMethodRepository> pharmacyShippingMethodRepositoryMock)
        {
            PharmacyShippingMethodRepositoryMock = pharmacyShippingMethodRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IPatientRepository> patientRepositoryMock)
        {
            PatientRepositoryMock = patientRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IShippingAddressRepository> shippingAddressRepositoryMock)
        {
            ShippingAddressRepositoryMock = shippingAddressRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPatientAttachmentRepository> patientAttachmentRepositoryMock)
        {
            PatientAttachmentRepositoryMock = patientAttachmentRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IAttachmentRepository> attachmentRepositoryMock)
        {
            AttachmentRepositoryMock = attachmentRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPasswordHasher<ApplicationUser>> passwordHasherMock)
        {
            PasswordHasherMock = passwordHasherMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<UserManager<ApplicationUser>> userManagerMock)
        {
            UserManagerMock = userManagerMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IUserServiceLinkRepository> userServiceLinkRepositoryMock)
        {
            UserServiceLinkRepositoryMock = userServiceLinkRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ClinicDbContext> clinicDbContextMock)
        {
            ClinicDbContextMock = clinicDbContextMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ILicenseInformationService> licenseInformationServiceMock)
        {
            LicenseInformationServiceMock = licenseInformationServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IBlobService> blobServiceMock)
        {
            BlobServiceMock = blobServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<AzureBlobStorageSettings> azureBlobStorageSettingsMock)
        {
            AzureBlobStorageSettingsMock = azureBlobStorageSettingsMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IUserSignatureRepository> userSignatureRepositoryMock)
        {
            UserSignatureRepositoryMock = userSignatureRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IUserOtpRepository> userOtpRepositoryMock)
        {
            UserOtpRepositoryMock = userOtpRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IRefreshTokenRepository> refreshTokenRepositoryMock)
        {
            RefreshTokenRepositoryMock = refreshTokenRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IJwtService> jwtServiceMock)
        {
            JwtServiceMock = jwtServiceMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IMessageRepository> messageRepositoryMock)
        {
            MessageRepositoryMock = messageRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IPatientAgendaService> patientAgendaServiceMock)
        {
            PatientAgendaServiceMock = patientAgendaServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPatientCreditCardService> patientCreditCardServiceMock)
        {
            PatientCreditCardServiceMock = patientCreditCardServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IAddressService> addressServiceMock)
        {
            AddressServiceMock = addressServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPatientAttachmentService> patientAttachmentServiceMock)
        {
            PatientAttachmentServiceMock = patientAttachmentServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IAttachmentService> attachmentServiceMock)
        {
            AttachmentServiceMock = attachmentServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IShippingAddressService> shippingAddressServiceMock)
        {
            ShippingAddressServiceMock = shippingAddressServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IMessageContentService> messageContentServiceMock)
        {
            MessageContentServiceMock = messageContentServiceMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<ISingletonService> singletonServiceMock)
        {
            SingletonServiceMock = singletonServiceMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IPharmacyShippingMethodService> pharmacyShippingMethodServiceMock)
        {
            PharmacyShippingMethodServiceMock = pharmacyShippingMethodServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IUserService> userServiceMock)
        {
            UserServiceMock = userServiceMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IProductRepository> productRepositoryMock)
        {
            ProductRepositoryMock = productRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IProductStrengthRepository> productStrengthRepositoryMock)
        {
            ProductStrengthRepositoryMock = productStrengthRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPriceListItemRepository> priceListItemRepositoryMock)
        {
            PriceListItemRepositoryMock = priceListItemRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IProductStatusRepository> productStatusRepositoryMock)
        {
            ProductStatusRepositoryMock = productStatusRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IProductTypeRepository> productTypeRepositoryMock)
        {
            ProductTypeRepositoryMock = productTypeRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IProductCategoryRepository> productCategoryRepositoryMock)
        {
            ProductCategoryRepositoryMock = productCategoryRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IProductWebFormRepository> productWebFormRepositoryMock)
        {
            ProductWebFormRepositoryMock = productWebFormRepositoryMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IBatchMessageRepository> batchMessageRepositoryMock)
        {
            BatchMessageRepositoryMock = batchMessageRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IBatchMessageRecipientRepository> batchMessageRecipientRepositoryMock)
        {
            BatchMessageRecipientRepositoryMock = batchMessageRecipientRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IConversationRepository> conversationRepositoryMock)
        {
            ConversationRepositoryMock = conversationRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IMessageContentRepository> messageContentRepositoryMock)
        {
            MessageContentRepositoryMock = messageContentRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ISmsService> smsServiceMock)
        {
            SmsServiceMock = smsServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPatientService> patientServiceMock)
        {
            PatientServiceMock = patientServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ILeadService> leadServiceMock)
        {
            LeadServiceMock = leadServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IMessageService> messageServiceMock)
        {
            MessageServiceMock = messageServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IHubContext<SmsHub>> hubContextMock)
        {
            HubContextMock = hubContextMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IAudioConverter> audioConverterMock)
        {
            AudioConverterMock = audioConverterMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IReminderRepository> reminderRepositoryMock)
        {
            ReminderRepositoryMock = reminderRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IReminderTypeRepository> reminderTypeRepositoryMock)
        {
            ReminderTypeRepositoryMock = reminderTypeRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IRecurrenceRuleRepository> recurrenceRuleRepositoryMock)
        {
            RecurrenceRuleRepositoryMock = recurrenceRuleRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ILeadReminderRepository> leadReminderRepositoryMock)
        {
            LeadReminderRepositoryMock = leadReminderRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPatientReminderRepository> patientReminderRepositoryMock)
        {
            PatientReminderRepositoryMock = patientReminderRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ILeadRepository> leadRepositoryMock)
        {
            LeadRepositoryMock = leadRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IBatchMessageRecipientService> batchMessageRecipientServiceMock)
        {
            BatchMessageRecipientServiceMock = batchMessageRecipientServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(IOptions<AzureBlobStorageSettings> azureBlobStorageOptions)
        {
            AzureBlobStorageOptions = azureBlobStorageOptions;
            return this;
        }
    }
}
