namespace SaasTaskManager.Core.Common;

public static class Utils
{
    public static string HashPassword(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Email and password must not be empty.");
        }
        
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var saltedPassword = email.ToLowerInvariant() + password;
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
        return Convert.ToBase64String(hashBytes);
    }
}