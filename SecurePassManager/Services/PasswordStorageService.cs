using System;
using System.Collections.Generic;
using System.IO;
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
        }

        public void AddPassword(string userId, PasswordEntry entry, string masterPassword)
        {
            if (!_userPasswords.ContainsKey(userId))
            {
                _userPasswords[userId] = new List<PasswordEntry>();
            }
            _userPasswords[userId].Add(entry);
            SavePasswords(masterPassword);
        }

        public void UpdatePassword(string userId, PasswordEntry entry, string masterPassword)
        {
            if (_userPasswords.ContainsKey(userId))
            {
                var existingEntry = _userPasswords[userId].Find(p => p.Id == entry.Id);
                if (existingEntry != null)
                {
                    existingEntry.Title = entry.Title;
                    existingEntry.Username = entry.Username;
                    existingEntry.Password = entry.Password;
                    existingEntry.Website = entry.Website;
                    existingEntry.UpdatedAt = DateTime.UtcNow;
                    SavePasswords(masterPassword);
                }
            }
        }

        public void DeletePassword(string userId, string id, string masterPassword)
        {
            if (_userPasswords.ContainsKey(userId))
            {
                _userPasswords[userId].RemoveAll(p => p.Id == id);
                SavePasswords(masterPassword);
            }
        }

        public PasswordEntry GetPassword(string userId, string id, string masterPassword)
        {
            LoadPasswords(masterPassword);
            return _userPasswords.ContainsKey(userId)
                ? _userPasswords[userId].Find(p => p.Id == id)
                : null;
        }

        public IEnumerable<PasswordEntry> GetAllPasswords(string userId, string masterPassword)
        {
            LoadPasswords(masterPassword);
            return _userPasswords.ContainsKey(userId)
                ? _userPasswords[userId].ToList()
                : new List<PasswordEntry>();
        }

        private void LoadPasswords(string masterPassword)
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string encryptedJson = File.ReadAllText(_filePath);
                    string decryptedJson = _encryptionService.Decrypt(encryptedJson, masterPassword);
                    _userPasswords = JsonSerializer.Deserialize<Dictionary<string, List<PasswordEntry>>>(decryptedJson)
                        ?? new Dictionary<string, List<PasswordEntry>>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading passwords: {ex.Message}");
                    _userPasswords = new Dictionary<string, List<PasswordEntry>>();
                }
            }
        }

        private void SavePasswords(string masterPassword)
        {
            string json = JsonSerializer.Serialize(_userPasswords);
            string encryptedJson = _encryptionService.Encrypt(json, masterPassword);
            File.WriteAllText(_filePath, encryptedJson);
        }
    }
}
