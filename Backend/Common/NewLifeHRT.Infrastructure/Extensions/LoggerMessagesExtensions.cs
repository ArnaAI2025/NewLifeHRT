using Microsoft.Extensions.Logging;

namespace NewLifeHRT.Infrastructure.Extensions
{
    public static partial class LoggerMessagesExtensions
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Tenant {TenantId} loaded successfully.")]
        public static partial void LogTenantLoaded(this ILogger logger, string tenantId);

        [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Error loading tenant {TenantId}: {ErrorMessage}")]
        public static partial void LogTenantLoadError(this ILogger logger, string tenantId, string errorMessage);

        [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Finished loading {TenantCount} tenants.")]
        public static partial void LogFinishedLoadingTenants(this ILogger logger, int tenantCount);

        [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Tenant {TenantId} already unloaded.")]
        public static partial void LogTenantAlreadyUnloaded(this ILogger logger, string tenantId);

        [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Finished unloading {TenantCount} tenants.")]
        public static partial void LogFinishedUnloadingTenants(this ILogger logger, int tenantCount);

        [LoggerMessage(EventId = 7, Level = LogLevel.Warning, Message = "Could not unload tenant {TenantId}.")]
        public static partial void LogCouldNotUnloadTenant(this ILogger logger, string tenantId);

        [LoggerMessage(EventId = 8, Level = LogLevel.Warning, Message = "Unknown tenant from host: {TenantId}.")]
        public static partial void LogUnknownTenantFromHost(this ILogger logger, string tenantId);

        [LoggerMessage(EventId = 9, Level = LogLevel.Information, Message = "Executing OnTenantLoading for {TenantName}.")]
        public static partial void LogExecutingOnTenantLoading(this ILogger logger, string tenantName);

        [LoggerMessage(EventId = 10, Level = LogLevel.Warning, Message = "Could not update tenant {TenantName}.")]
        public static partial void LogCouldNotUpdateTenant(this ILogger logger, string tenantName);

        [LoggerMessage(EventId = 11, Level = LogLevel.Information, Message = "Executing OnTenantLoaded for {TenantName}.")]
        public static partial void LogExecutingOnTenantLoaded(this ILogger logger, string tenantName);

        [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Loaded tenant {TenantName}.")]
        public static partial void LogLoadedTenant(this ILogger logger, string tenantName);

        [LoggerMessage(EventId = 13, Level = LogLevel.Information, Message = "Unloaded tenant {TenantName}.")]
        public static partial void LogUnloadedTenant(this ILogger logger, string tenantName);
        [LoggerMessage(EventId = 14, Level = LogLevel.Debug, Message = "The tenant was already loaded and won't be loaded again. [TenantId: {tenantId}]")]
        public static partial void LogTenantAlreadyLoaded(this ILogger logger, string tenantId);

        [LoggerMessage(15, LogLevel.Error, "Unable to set tenant context. [TenantId: {tenantId}]")]
        public static partial void LogUnableToSetTenantContext(this ILogger logger, string tenantId);
    }
}
