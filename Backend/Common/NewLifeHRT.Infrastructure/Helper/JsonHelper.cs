using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NewLifeHRT.Infrastructure.Helper
{
    public static class JsonHelper
    {

        //To Deserialize and Log the JSON Object 
        private static readonly ILogger Logger;

        static JsonHelper()
        {
            var factory = new LoggerFactory();
            Logger = factory.CreateLogger("JsonHelper");
        }

        public static T DeserializeSafe<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogDeserializationErrors<T>(json, ex);
                throw;
            }
        }

        private static void LogDeserializationErrors<T>(string raw, Exception ex)
        {
            Logger.LogError(ex, "Failed to deserialize JSON payload");

            try
            {
                var payload = JObject.Parse(raw);

                var jsonFields = payload.Properties().Select(p => p.Name).ToList();
                var dtoFields = typeof(T).GetProperties().Select(p => p.Name).ToList();

                var extra = jsonFields.Except(dtoFields, StringComparer.OrdinalIgnoreCase).ToList();
                var missing = dtoFields.Except(jsonFields, StringComparer.OrdinalIgnoreCase).ToList();

                Logger.LogWarning("Extra JSON fields: {Extra}", extra);
                Logger.LogWarning("Missing DTO fields: {Missing}", missing);
            }
            catch (Exception inner)
            {
                Logger.LogError(inner, "Failed to compare JSON and DTO.");
            }
        }

        public static string ExtractPropertySafe(string json, string propertyName)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);

                return FindPropertyRecursive(doc.RootElement, propertyName);
            }
            catch
            {
                return null;
            }
        }

        private static string FindPropertyRecursive(JsonElement element, string propertyName)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                            return prop.Value.ToString();

                        var nested = FindPropertyRecursive(prop.Value, propertyName);
                        if (nested != null)
                            return nested;
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        var nested = FindPropertyRecursive(item, propertyName);
                        if (nested != null)
                            return nested;
                    }
                    break;
            }

            return null;
        }

    }
}
