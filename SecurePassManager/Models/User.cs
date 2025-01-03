using System;

namespace SecurePassManager.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; }
        public string EncryptedCredentials { get; set; }
        public string CredentialsSalt { get; set; }
    }
}