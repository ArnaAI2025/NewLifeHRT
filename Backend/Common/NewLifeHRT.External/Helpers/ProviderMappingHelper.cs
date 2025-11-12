using NewLifeHRT.Domain.Enums;
using System.Reflection;

namespace NewLifeHRT.External.Helpers
{
    public static class ProviderMappingHelper
    {

        public static bool IsPickupShipping(string shippingMethod)
        {
            if (string.IsNullOrWhiteSpace(shippingMethod))
                return false;

            return shippingMethod.Trim()
                .Equals("pick up", StringComparison.OrdinalIgnoreCase);
        }

        public static string NormalizePhone(string? number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return null;

            return new string(number.Where(char.IsDigit).ToArray());
        }

        public static char GetGenderChar(this GenderEnum gender)
        {
            return gender switch
            {
                GenderEnum.Male => 'M',
                GenderEnum.Female => 'F',
                _ => 'U'
            };
        }

        public static string GetWellsShippingMethodName(this string shippingMethod)
        {
            switch (shippingMethod.ToLower())
            {
                case "pick up":
                    return "Pick";
                default:
                    return "FEXP";
            }
        }
    }
}
