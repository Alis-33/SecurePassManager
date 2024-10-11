using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SecurePassManager.Models;

namespace SecurePassManager.Services
{
    public class PasswordStorageService
    {
        private readonly EncryptionService _encryptionService;
        private readonly string _filePath;
        private Dictionary<string, List<PasswordEntry>> _userPasswords = new Dictionary<string, List<PasswordEntry>>();

        public PasswordStorageService(EncryptionService encryptionService, string filePath)
        {
            _encryptionService = encryptionService;
            _filePath = filePath;
            LoadPasswords();
        }
        public void AddPassword(string userId, PasswordEntry entry)
        {
            if (!_userPasswords.ContainsKey(userId))
            {
                _userPasswords[userId] = new List<PasswordEntry>();
            }
            _userPasswords[userId].Add(entry);
            SavePasswords();
        }

        public void UpdatePassword(string userId, PasswordEntry entry)
        {
            if (_userPasswords.ContainsKey(userId))
            {
                var existingEntry = _userPasswords[userId].FirstOrDefault(p => p.Id == entry.Id);
                if (existingEntry != null)
                {
                    existingEntry.Title = entry.Title;
                    existingEntry.Username = entry.Username;
                    existingEntry.Password = entry.Password;
                    existingEntry.Website = entry.Website;
                    existingEntry.UpdatedAt = DateTime.UtcNow;
                    SavePasswords();
                }
            }
        }

        public void DeletePassword(string userId, string id)
        {
            if (_userPasswords.ContainsKey(userId))
            {
                _userPasswords[userId].RemoveAll(p => p.Id == id);
                SavePasswords();
            }
        }

        public PasswordEntry GetPassword(string userId, string id)
        {
            return _userPasswords.ContainsKey(userId) 
                ? _userPasswords[userId].FirstOrDefault(p => p.Id == id) 
                : null;
        }

        public IEnumerable<PasswordEntry> GetAllPasswords(string userId)
        {
            return _userPasswords.ContainsKey(userId) 
                ? _userPasswords[userId].ToList() 
                : new List<PasswordEntry>();
        }

        private void LoadPasswords()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string encryptedJson = File.ReadAllText(_filePath);
                    string decryptedJson = _encryptionService.Decrypt(encryptedJson);
                    _userPasswords = JsonSerializer.Deserialize<Dictionary<string, List<PasswordEntry>>>(decryptedJson) 
                                     ?? new Dictionary<string, List<PasswordEntry>>();
                }
                catch (Exception ex)
                {
                    // Log the error and initialize with an empty dictionary
                    Console.WriteLine($"Error loading passwords: {ex.Message}");
                    _userPasswords = new Dictionary<string, List<PasswordEntry>>();
                }
            }
        }

        private void SavePasswords()
        {
            string json = JsonSerializer.Serialize(_userPasswords);
            string encryptedJson = _encryptionService.Encrypt(json);
            File.WriteAllText(_filePath, encryptedJson);
        }
    }
}