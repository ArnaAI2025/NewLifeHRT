using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure
{
    public static class OtpHelper
    {
        public static string GenerateSecureOtp()
        {
            int otp;
            var bytes = new byte[4];

            do
            {
                RandomNumberGenerator.Fill(bytes);
                otp = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
                otp %= 1000000;
            } while (otp < 100000);

            return otp.ToString();
        }
    }
}
