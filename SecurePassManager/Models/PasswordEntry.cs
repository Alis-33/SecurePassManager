using System;

namespace SecurePassManager.Models
{
    public class PasswordEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Title { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Website { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}