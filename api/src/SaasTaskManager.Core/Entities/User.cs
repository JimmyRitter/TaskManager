using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Core.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string HashedPassword { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    public static User Create(string email, string name, string plainTextPassword)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            Name = name,
            HashedPassword = Utils.HashPassword(email, plainTextPassword),
            CreatedAt = DateTime.UtcNow,
            IsEmailVerified = false,
            IsActive = true
        };
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiresAt = null;
    }

    public void SetEmailVerificationToken(string token, TimeSpan expirationPeriod)
    {
        EmailVerificationToken = token;
        EmailVerificationTokenExpiresAt = DateTime.UtcNow.Add(expirationPeriod);
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}