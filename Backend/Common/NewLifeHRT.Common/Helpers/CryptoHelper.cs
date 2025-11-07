using System;
using System.Text;
using System.Security.Cryptography;

namespace NewLifeHRT.Common.Helpers
{
    public static class CryptoHelper
    {
        public static string Encrypt(string plainText, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            var encryptor = aes.CreateEncryptor();
            var inputBytes = Encoding.UTF8.GetBytes(plainText);
            var outputBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Convert.ToBase64String(outputBytes);
        }

        public static string Decrypt(string encryptedText, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            var decryptor = aes.CreateDecryptor();
            var inputBytes = Convert.FromBase64String(encryptedText);
            var outputBytes = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Encoding.UTF8.GetString(outputBytes);
        }
    }

}
