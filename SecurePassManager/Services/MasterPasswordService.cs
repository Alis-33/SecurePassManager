using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SecurePassManager.Models;

namespace SecurePassManager.Services
{
    public class MasterPasswordService
    {
        private readonly string _usersFilePath;
        private readonly List<User> _users = new List<User>();

        public MasterPasswordService(string usersFilePath)
        {
            _usersFilePath = usersFilePath;
            if (File.Exists(_usersFilePath))
            {
                string json = File.ReadAllText(_usersFilePath);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
        }

        public bool VerifyMasterPassword(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;

            try
            {
                var credentialsSalt = Convert.FromBase64String(user.CredentialsSalt);
                var decryptedCredentials = DecryptCredentials(user.EncryptedCredentials, password, credentialsSalt);
                var credentials = JsonSerializer.Deserialize<UserCredentials>(decryptedCredentials);
                
                var hashedPassword = HashPassword(password, credentials.Salt);
                return hashedPassword == credentials.PasswordHash;
            }
            catch
            {
                return false;
            }
        }

        public (bool success, string? userId) CreateUser(string username, string password)
        {
            if (_users.Any(u => u.Username == username))
            {
                return (false, null);
            }

            // Generate salt for the password hash
            var passwordSalt = GenerateSalt();
            var hashedPassword = HashPassword(password, Convert.ToBase64String(passwordSalt));

            // Create and encrypt the credentials
            var credentials = new UserCredentials
            {
                PasswordHash = hashedPassword,
                Salt = Convert.ToBase64String(passwordSalt)
            };
            
            var credentialsJson = JsonSerializer.Serialize(credentials);
            var credentialsSalt = GenerateSalt();
            var encryptedCredentials = EncryptCredentials(credentialsJson, password, credentialsSalt);

            var newUser = new User
            {
                Username = username,
                EncryptedCredentials = encryptedCredentials,
                CredentialsSalt = Convert.ToBase64String(credentialsSalt)
            };

            _users.Add(newUser);
            SaveUsers();

            return (true, newUser.Id);
        }

        private string EncryptCredentials(string credentials, string password, byte[] salt)
        {
            using var aes = Aes.Create();
            var key = DeriveKey(password, salt);
            aes.Key = key;
            aes.GenerateIV();

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(credentials);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        private string DecryptCredentials(string encryptedCredentials, string password, byte[] salt)
        {
            var cipherBytes = Convert.FromBase64String(encryptedCredentials);
            using var aes = Aes.Create();
            var key = DeriveKey(password, salt);
            
            var iv = new byte[aes.BlockSize / 8];
            var cipher = new byte[cipherBytes.Length - iv.Length];
            
            Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, iv.Length, cipher, 0, cipher.Length);
            
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream(cipher);
            using var decryptor = aes.CreateDecryptor();
            using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);
            
            return reader.ReadToEnd();
        }

        private byte[] DeriveKey(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 210000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        private string HashPassword(string password, string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password, 
                Convert.FromBase64String(salt), 
                210000, 
                HashAlgorithmName.SHA256);
            
            return Convert.ToBase64String(pbkdf2.GetBytes(32));
        }

        private byte[] GenerateSalt()
        {
            var salt = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        private void SaveUsers()
        {
            var json = JsonSerializer.Serialize(_users);
            File.WriteAllText(_usersFilePath, json);
        }

        private class UserCredentials
        {
            public string PasswordHash { get; set; }
            public string Salt { get; set; }
        }
    }
}