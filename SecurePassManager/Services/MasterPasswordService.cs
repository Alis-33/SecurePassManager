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
        private List<User> _users = new List<User>();

        public MasterPasswordService(string usersFilePath)
        {
            _usersFilePath = usersFilePath;
            LoadUsers();
        }

        public bool VerifyMasterPassword(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;

            var hashedPassword = HashPassword(password, user.Salt);
            return hashedPassword == user.PasswordHash;
        }

        public (bool success, string? userId) CreateUser(string username, string password)
        {
            if (_users.Any(u => u.Username == username))
            {
                return (false, null); // Username already exists
            }

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(password, Convert.ToBase64String(salt));

            var newUser = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Salt = Convert.ToBase64String(salt)
            };

            _users.Add(newUser);
            SaveUsers();

            return (true, newUser.Id);
        }

        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 210000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private void LoadUsers()
        {
            if (File.Exists(_usersFilePath))
            {
                string json = File.ReadAllText(_usersFilePath);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
        }

        private void SaveUsers()
        {
            string json = JsonSerializer.Serialize(_users);
            File.WriteAllText(_usersFilePath, json);
        }
    }
}