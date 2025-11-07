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
        public const string JwtKey = "Jwt:Key";
        public const string TwilioSettings = "Twilio";
        public const string AzureBlobStorageSettings = "AzureBlobStorage";
        public const string ClinicInformationSettings = "ClinicInformationSettings";

        public const string JWTSettings = "JWTSettings";
        public const string Authentication = "Authentication";
        public const string TemplateSettings = "TemplateSettings";

        public static class ConnectionStringKeys
        {
            public const string HospitalDatabase = "HospitalDatabase";
        }
    }
}
