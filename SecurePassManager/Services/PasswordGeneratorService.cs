using System;
using System.Linq;
using System.Security.Cryptography;

namespace SecurePassManager.Services
{
    public class PasswordGeneratorService
    {
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumericChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_-+=<>?";

        public string GeneratePassword(int length = 16, bool includeLowercase = true, bool includeUppercase = true, bool includeNumeric = true, bool includeSpecial = true)
        {
            var charSet = "";
            if (includeLowercase) charSet += LowercaseChars;
            if (includeUppercase) charSet += UppercaseChars;
            if (includeNumeric) charSet += NumericChars;
            if (includeSpecial) charSet += SpecialChars;

            if (string.IsNullOrEmpty(charSet))
            {
                throw new ArgumentException("At least one character set must be selected.");
            }

            var result = new char[length];
            var random = new RNGCryptoServiceProvider();

            for (int i = 0; i < length; i++)
            {
                var randomByte = new byte[1];
                random.GetBytes(randomByte);
                result[i] = charSet[randomByte[0] % charSet.Length];
            }

            return new string(result);
        }
    }
}