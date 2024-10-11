using System;
using System.Text.RegularExpressions;

namespace SecurePassManager.Services
{
    public class ValidationService
    {
        public bool IsValidPassword(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Password cannot be empty.";
                return false;
            }

            if (password.Length < 12)
            {
                errorMessage = "Password must be at least 12 characters long.";
                return false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasNumber.IsMatch(password))
            {
                errorMessage = "Password must contain at least one number.";
                return false;
            }
            else if (!hasUpperChar.IsMatch(password))
            {
                errorMessage = "Password must contain at least one upper case letter.";
                return false;
            }
            else if (!hasLowerChar.IsMatch(password))
            {
                errorMessage = "Password must contain at least one lower case letter.";
                return false;
            }
            else if (!hasSymbols.IsMatch(password))
            {
                errorMessage = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use .NET's built-in email address check
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}