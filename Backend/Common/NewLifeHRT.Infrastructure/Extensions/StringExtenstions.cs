using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Extensions
{
    public static class StringExtenstions
    {
        public static string ToPascalCaseClean(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove all special characters except letters and numbers
            string cleaned = Regex.Replace(input, "[^a-zA-Z0-9 ]+", " ");

            // Split by spaces
            var words = cleaned
                .Split(" ", StringSplitOptions.RemoveEmptyEntries);

            StringBuilder result = new StringBuilder();

            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    result.Append(
                        char.ToUpper(word[0], CultureInfo.InvariantCulture) +
                        (word.Length > 1 ? word.Substring(1).ToLower() : "")
                    );
                }
            }

            return result.ToString();
        }

    }
}
