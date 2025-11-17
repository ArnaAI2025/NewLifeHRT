using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.StartupSection
{
    public static class ApplicationServiceInitializer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClinicServiceService, ClinicServiceService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductStatusService, ProductStatusService>();
            services.AddScoped<IProductWebFormService, ProductWebFormService>();
            services.AddScoped<IProductStrengthService, ProductStrengthService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IVisitTypeService, VisitTypeService>();
            services.AddScoped<IAgendaService, AgendaService>();
            services.AddScoped<ICounselorNoteService, CounselorNoteService>();
            services.AddScoped<IMedicalRecommendationService, MedicalRecommendationService>();
            services.AddScoped<IMedicationTypeService, MedicationTypeService>();
            services.AddScoped<IFollowUpLabTestService, FollowUpLabTestService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IPatientAttachmentService, PatientAttachmentService>();
            services.AddScoped<IPharmacyService, PharmacyService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IPriceListItemService, PriceListItemService>();
            services.AddScoped<ILifeFileDrugFormService, LifeFileDrugFormService>();
            services.AddScoped<ILifeFileQuantityUnitService, LifeFileQuantityUnitService>();
            services.AddScoped<ILifeFileScheduleCodeService, LifeFileScheduleCodeService>();
            services.AddScoped<ICommisionRateService, CommisionRateService>();
            services.AddScoped<IPatientCreditCardService, PatientCreditCardService>();
            services.AddScoped<IPatientAgendaService, PatientAgendaService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IDocumentCategoryService, DocumentCategoryService>();
            services.AddScoped<IProposalService, ProposalService>();
            services.AddScoped<IProposalDetailService, ProposalDetailService>();
            services.AddScoped<IShippingAddressService, ShippingAddressService>();  
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IPharmacyShippingMethodService, PharmacyShippingMethodService>();
            services.AddScoped<IShippingMethodService, ShippingMethodService>();
            services.AddScoped<IAppointmentModeService, AppointmentModeService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<ISlotService, SlotService>();
            services.AddScoped<IHolidayService, HolidayService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IPoolService, PoolService>();
            services.AddScoped<IPoolDetailService, PoolDetailService>();
            services.AddScoped<ITimezoneService, TimezoneService>();
            services.AddScoped<IBatchMessageService, BatchMessageService>();
            services.AddScoped<IBatchMessageRecipientService, BatchMessageRecipientService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMessageContentService, MessageContentService>(); 
            services.AddScoped<IPharmacyConfigurationService, PharmacyConfigurationService>();
            services.AddScoped<IOrderProcessingApiTrackingService, OrderProcessingApiTrackingService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<ILicenseInformationService, LicenseInformationService>();
            services.AddScoped<ICommissionsPayableService, CommissionsPayableService>(); 
            services.AddScoped<ICommissionsPayableDetailService, CommissionsPayableDetailService>(); 
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IOrderProductsRefillService, OrderProductsRefillService>();
            services.AddScoped<IOrderProductScheduleService, OrderProductScheduleService>();
            return services;
        }
    }
}
