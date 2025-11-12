using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using NewLifeHRT.Jobs.Scheduler.Services;
using System;
using System.Net.Http;
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
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;

        public WeeklyCommissionJob(
            IServiceScopeFactory scopeFactory,
            ILogger<WeeklyCommissionJob> logger,
            IMultiTenantContextAccessor<MultiTenantInfo> multiTenantContextAccessor,
            IWeeklyCommissionService weeklyCommissionService,
            ITemplateContentGenerator templateContentGenerator,
            IPdfConverter pdfConverter,
            IOptions<AzureBlobStorageSettings> azureBlobStorageSettings
        ) : base(scopeFactory, logger)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _weeklyCommissionService = weeklyCommissionService;
            _logger = logger;
            _templateContentGenerator = templateContentGenerator;
            _pdfConverter = pdfConverter;
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
        }

        protected override async Task ExecuteForTenantAsync(CancellationToken cancellationToken)
        {
            var tenantName = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? "Unknown Tenant";

            try
            {
                _logger.LogInformation("WeeklyCommissionJob started for tenant: {Tenant}", tenantName);

                _logger.LogInformation("Fetching ready-to-generate commission data for tenant: {Tenant}", tenantName);
                await _weeklyCommissionService.GetReadyToGenerateCommission();

                _logger.LogInformation("Fetching counselors in current pool for tenant: {Tenant}", tenantName);
                var poolDetails = await _weeklyCommissionService.GetCounselorsInCurrentPoolAsync();

                string logo = $"{_azureBlobStorageSettings.ContainerSasUrl}/clinic1/logo.png?{_azureBlobStorageSettings.SasToken}";
                if (!string.IsNullOrEmpty(logo))
                {
                    _logger.LogInformation("Converting logo URL to Base64 for tenant: {Tenant}", tenantName);
                    logo = await ConvertImageUrlToBase64Async(logo);
                }

                foreach (var item in poolDetails)
                {
                    _logger.LogInformation("Generating commission report for CounselorId: {CounselorId}, Tenant: {Tenant}", item.CounselorId, tenantName);

                    var response = await _weeklyCommissionService.GetCommissionReportByCounselor(item.CounselorId);
                    response.ReportRange = $"{item.Pool.FromDate:dd-MMM-yyyy} to {item.Pool.ToDate:dd-MMM-yyyy}";
                    response.Logo = logo;
                    response.TemplatePath = "SalesPersonWiseCommissionReportTemplate.cshtml";

                    _logger.LogDebug("Rendering HTML template for CounselorId: {CounselorId}, Tenant: {Tenant}", item.CounselorId, tenantName);
                    var htmlContent = _templateContentGenerator.GetTemplateContent(response);

                    _logger.LogDebug("Converting HTML to PDF for CounselorId: {CounselorId}, Tenant: {Tenant}", item.CounselorId, tenantName);
                    var pdfResult = _pdfConverter.ConvertToPdf(htmlContent);

                    _logger.LogInformation("Successfully generated PDF for CounselorId: {CounselorId}, Tenant: {Tenant}", item.CounselorId, tenantName);
                }

                _logger.LogInformation("WeeklyCommissionJob completed successfully for tenant: {Tenant}", tenantName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WeeklyCommissionJob failed for tenant: {Tenant}. Error: {Message}", tenantName, ex.Message);
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
