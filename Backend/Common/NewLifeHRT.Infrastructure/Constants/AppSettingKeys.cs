using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Constants
{
    public static class AppSettingKeys
    {
        
        public const string MultiTenancySettings = "MultiTenancy";
        public const string TwilioSettings = "Twilio";
        public const string AzureBlobStorageSettings = "AzureBlobStorage";
        public const string ClinicInformationSettings = "ClinicInformationSettings";

        public const string JWTSettings = "JWTSettings";
        public const string Authentication = "Authentication";
        public const string TemplateSettings = "TemplateSettings";
        public const string ApplicationInsights = "ApplicationInsights";
        public const string SecuritySettings = "SecuritySettings";
        public const string AISettings = "AISettings";
        public static class ConnectionStringKeys
        {
            public const string HospitalDatabase = "HospitalDatabase";
            public const string HangfireConnection = "HangfireConnection";
        }
    }
}
