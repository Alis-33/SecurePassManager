using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Maui.Storage;

namespace SecurePassManager.Services
{
    public class EncryptionService
    {
        private const int Iterations = 100000;
        private const int KeySize = 256;
        private const int BlockSize = 128;
        private const int SaltSize = 32;
        private const string SaltFileName = "encryption.salt";
        private const string KeyFileName = "encryption.key";

        private readonly byte[] _salt;
        private readonly string _encryptionKey;

        public EncryptionService()
        {
            _salt = GetOrCreateSalt();
            _encryptionKey = GetOrCreateEncryptionKey();
        }

        private byte[] GetOrCreateSalt()
        {
            string saltPath = Path.Combine(FileSystem.AppDataDirectory, SaltFileName);
            if (File.Exists(saltPath))
            {
                return File.ReadAllBytes(saltPath);
            }
            else
            {
                byte[] salt = new byte[SaltSize];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                File.WriteAllBytes(saltPath, salt);
                return salt;
            }
        }

        private string GetOrCreateEncryptionKey()
        {
            string keyPath = Path.Combine(FileSystem.AppDataDirectory, KeyFileName);
            if (File.Exists(keyPath))
            {
                return File.ReadAllText(keyPath);
            }
            else
            {
                byte[] keyBytes = new byte[32]; // 256 bits
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(keyBytes);
                }
                string key = Convert.ToBase64String(keyBytes);
                File.WriteAllText(keyPath, key);
                return key;
            }
        }

        public string Encrypt(string plainText)
        {
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var key = new Rfc2898DeriveBytes(_encryptionKey, _salt, Iterations, HashAlgorithmName.SHA256))
                {
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                }

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var key = new Rfc2898DeriveBytes(_encryptionKey, _salt, Iterations, HashAlgorithmName.SHA256))
                {
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                }

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(buffer))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}