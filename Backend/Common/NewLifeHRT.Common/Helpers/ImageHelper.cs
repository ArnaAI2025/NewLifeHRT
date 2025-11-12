using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Downloads an image from the specified URL and converts it to a Base64 data URI string.
        /// </summary>
        public static async Task<string> ConvertImageUrlToBase64Async(string imageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
            }
        }
    }

}
