using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Helpers;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly ClinicDbContext _clinicDbContext;
        private readonly SecuritySettings _settings;
        public OrderProcessingService(ClinicDbContext clinicDbContext, IOptions<SecuritySettings> options) 
        {
            _clinicDbContext = clinicDbContext;
            _settings = options.Value;
        }

        /// <summary>
        /// Retrieves and decrypts the configuration settings for a specific pharmacy.
        /// </summary>
        public async Task<(string, Dictionary<string, string>)> GetDecryptedConfigAsync(Guid pharmacyId, CancellationToken cancellationToken)
        {
            var configEntity = await _clinicDbContext.PharmacyConfigurations
                .Include(pc => pc.ConfigurationData)
                    .ThenInclude(cd => cd.IntegrationKey)
                .Include(pc => pc.IntegrationType)
                .FirstOrDefaultAsync(pc => pc.PharmacyId == pharmacyId, cancellationToken);

            if (configEntity == null)
                return (string.Empty, new Dictionary<string, string>());

            var encryptionKey = _settings.Key;
            var iv = _settings.IV;

            return (configEntity.IntegrationType.Type, configEntity.ConfigurationData.ToDictionary(
                cd => cd.IntegrationKey.KeyName,
                cd => CryptoHelper.Decrypt(cd.Value, encryptionKey, iv)
            ));
        }

        /// <summary>
        /// Retrieves all active orders that are pending LifeFile processing.
        /// </summary>
        public async Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken)
        {
            var pendingOrders = await _clinicDbContext.Orders
                .Include(o => o.Pharmacy)
                .Where(o => o.IsActive
                    && o.IsReadyForLifeFile == true
                    && o.Status == OrderStatus.LifeFileProcessing)
                .ToListAsync(cancellationToken);

            return pendingOrders;
        }
    }
}
