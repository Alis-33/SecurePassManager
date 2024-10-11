using Microsoft.Extensions.Logging;
using System.Text;

namespace SecurePassManager.Services
{
    public class SecurityLogger
    {
        private readonly ILogger<SecurityLogger> _logger;

        public SecurityLogger(ILogger<SecurityLogger> logger)
        {
            _logger = logger;
        }

        public void LogLoginAttempt(bool success, string username)
        {
            var message = new StringBuilder()
                .Append("Login attempt ")
                .Append(success ? "successful" : "failed")
                .Append(" for user: ")
                .Append(username);

            if (success)
                _logger.LogInformation(message.ToString());
            else
                _logger.LogWarning(message.ToString());
        }

        public void LogPasswordCreated(string username)
        {
            _logger.LogInformation("New password created for user: {Username}", username);
        }

        public void LogPasswordUpdated(string username)
        {
            _logger.LogInformation("Password updated for user: {Username}", username);
        }

        public void LogPasswordDeleted(string username)
        {
            _logger.LogInformation("Password deleted for user: {Username}", username);
        }

        public void LogSecurityEvent(string eventDescription)
        {
            _logger.LogInformation("Security event: {EventDescription}", eventDescription);
        }

        public void LogSecurityWarning(string warningDescription)
        {
            _logger.LogWarning("Security warning: {WarningDescription}", warningDescription);
        }
    }
}