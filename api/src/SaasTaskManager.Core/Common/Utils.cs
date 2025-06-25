using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SaasTaskManager.Core.Common;

public static class Utils
{
    public static string HashPassword(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Email and password must not be empty.");
        }
        
        using var sha256 = SHA256.Create();
        var saltedPassword = email.ToLowerInvariant() + password;
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Validates password strength according to security requirements
    /// </summary>
    /// <param name="password">The password to validate</param>
    /// <param name="userInfo">User information to check against (email, name)</param>
    /// <returns>Validation result with error messages if invalid</returns>
    public static PasswordValidationResult ValidatePasswordStrength(string password, string? userInfo = null)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required.");
            return new PasswordValidationResult(false, errors);
        }

        // Minimum length
        if (password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters long.");
        }

        // Maximum length (prevent DoS attacks)
        if (password.Length > 128)
        {
            errors.Add("Password must not exceed 128 characters.");
        }

        // Uppercase letter
        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            errors.Add("Password must contain at least one uppercase letter.");
        }

        // Lowercase letter
        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            errors.Add("Password must contain at least one lowercase letter.");
        }

        // Digit
        if (!Regex.IsMatch(password, @"\d"))
        {
            errors.Add("Password must contain at least one number.");
        }

        // Special character
        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
        {
            errors.Add("Password must contain at least one special character.");
        }

        // Check if password contains user information
        if (!string.IsNullOrWhiteSpace(userInfo))
        {
            var userInfoLower = userInfo.ToLowerInvariant();
            var passwordLower = password.ToLowerInvariant();
            
            if (passwordLower.Contains(userInfoLower) || userInfoLower.Contains(passwordLower))
            {
                errors.Add("Password must not contain user information.");
            }
        }

        // Common password patterns
        if (IsCommonPassword(password))
        {
            errors.Add("Password is too common. Please choose a stronger password.");
        }

        return new PasswordValidationResult(errors.Count == 0, errors);
    }

    private static bool IsCommonPassword(string password)
    {
        // Simple list of common passwords - in production, use a comprehensive list
        var commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "password", "123456", "123456789", "qwerty", "abc123", "password123",
            "admin", "letmein", "welcome", "monkey", "dragon", "1234567", "password1"
        };

        return commonPasswords.Contains(password);
    }
}

public record PasswordValidationResult(bool IsValid, List<string> Errors);