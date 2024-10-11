using System.Text.RegularExpressions;

namespace SecurePassManager.Services
{
    public class PasswordStrengthService
    {
        public enum StrengthLevel
        {
            VeryWeak,
            Weak,
            Medium,
            Strong,
            VeryStrong
        }

        public StrengthLevel CalculatePasswordStrength(string password)
        {
            int score = 0;

            if (string.IsNullOrEmpty(password))
                return StrengthLevel.VeryWeak;

            // Award points for length
            if (password.Length > 8)
                score++;
            if (password.Length > 12)
                score++;
            if (password.Length > 16)
                score++;

            // Award points for complexity
            if (Regex.IsMatch(password, @"[a-z]"))
                score++;
            if (Regex.IsMatch(password, @"[A-Z]"))
                score++;
            if (Regex.IsMatch(password, @"[0-9]"))
                score++;
            if (Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
                score++;

            return score switch
            {
                0 => StrengthLevel.VeryWeak,
                1 or 2 => StrengthLevel.Weak,
                3 or 4 => StrengthLevel.Medium,
                5 or 6 => StrengthLevel.Strong,
                _ => StrengthLevel.VeryStrong
            };
        }
    }
}