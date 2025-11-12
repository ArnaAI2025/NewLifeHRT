using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Interfaces
{
    public interface ISmsService
    {
        Task<string> SendSmsAsync(string to, string message);
        Task<(byte[] content, string contentType)> GetTwilioMediaAsync(string mediaUrl);
    }
}
