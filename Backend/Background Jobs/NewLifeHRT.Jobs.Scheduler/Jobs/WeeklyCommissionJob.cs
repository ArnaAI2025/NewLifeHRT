using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Jobs.Scheduler.Interface;
using NewLifeHRT.Jobs.Scheduler.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Jobs
{
    public class WeeklyCommissionJob : MultiTenantJobBase<WeeklyCommissionJob>
    {
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _multiTenantContextAccessor;
        private readonly ILogger<WeeklyCommissionJob> _logger;
        private readonly IWeeklyCommissionService _weeklyCommissionService;
        private readonly ITemplateContentGenerator _templateContentGenerator;
        private readonly IPdfConverter _pdfConverter;
        private readonly ClinicInformationSettings _clinicInformationSettings;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;

        public WeeklyCommissionJob(
            IServiceScopeFactory scopeFactory,
            ILogger<WeeklyCommissionJob> logger,
            IMultiTenantContextAccessor<MultiTenantInfo> multiTenantContextAccessor,
            IWeeklyCommissionService weeklyCommissionService,
            ITemplateContentGenerator templateContentGenerator,
            IPdfConverter pdfConverter,
            IOptions<AzureBlobStorageSettings> azureBlobStorageSettings,
            IOptions<ClinicInformationSettings> clinicInformationSettings
        ) : base(scopeFactory, logger)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _weeklyCommissionService = weeklyCommissionService;
            _logger = logger;
            _templateContentGenerator = templateContentGenerator;
            _pdfConverter = pdfConverter;
        }

        protected override async Task ExecuteForTenantAsync(CancellationToken cancellationToken)
        {
            try
            {
                var tenantName = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? "Unknown Tenant";
                _logger.LogInformation($"Starting WeeklyCommissionJob for tenant: {tenantName}");
                await _weeklyCommissionService.GetReadyToGenerateCommission();
                var poolDetails = await _weeklyCommissionService.GetCounselorsInCurrentPoolAsync();
                string logo = "https://newlifedocument.blob.core.windows.net/nlr-patientdocuments/clinic1/logo.png?sp=racwl&st=2025-08-04T14:03:46Z&se=2025-12-30T22:18:46Z&spr=https&sv=2024-11-04&sr=c&sig=2C4PNjuLETPFwBVcp82qLC0h7f8pnGLEmpW3Efq9ItQ%3D";
                if (logo != null)
                {
                    logo = await ConvertImageUrlToBase64Async(logo);
                }
                foreach (var item in poolDetails)
                {
                    var response = await _weeklyCommissionService.GetCommissionReportByCounselor(item.CounselorId);
                    response.ReportRange = $"{item.Pool.FromDate:dd-MMM-yyyy} to {item.Pool.ToDate:dd-MMM-yyyy}";
                    response.Logo = logo;
                    response.TemplatePath = "SalesPersonWiseCommissionReportTemplete.cshtml";
                    var res = _templateContentGenerator.GetTemplateContent(response);
                    var pdfResult = _pdfConverter.ConvertToPdf(res);
                }
                _logger.LogInformation($"WeeklyCommissionJob completed for tenant: {tenantName}");
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<string> ConvertImageUrlToBase64Async(string imageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
            }
        }

    }
}
