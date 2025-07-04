using FluentAssertions;
using SaasTaskManager.Core.Entities;

namespace SaasTaskManager.Tests.Entities;

public class UserTests
{
    #region Create Method Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateUserWithCorrectProperties()
    {
        // Arrange
        var email = "test@example.com";
        var name = "Test User";
        var password = "TestPassword123!";

        // Act
        var user = User.Create(email, name, password);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(email.ToLowerInvariant());
        user.Name.Should().Be(name);
        user.HashedPassword.Should().NotBe(password); // Should be hashed
        user.HashedPassword.Should().NotBeNullOrWhiteSpace();
        user.IsEmailVerified.Should().BeFalse();
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.LastLoginAt.Should().BeNull();
        user.EmailVerificationToken.Should().BeNull();
        user.EmailVerificationTokenExpiresAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithDifferentData_ShouldCreateDistinctUsers()
    {
        // Act
        var user1 = User.Create("user1@example.com", "User One", "Password123!");
        var user2 = User.Create("user2@example.com", "User Two", "Password456!");

        // Assert
        user1.Id.Should().NotBe(user2.Id);
        user1.Email.Should().NotBe(user2.Email);
        user1.Name.Should().NotBe(user2.Name);
        user1.HashedPassword.Should().NotBe(user2.HashedPassword);
    }

    [Fact]
    public void Create_ShouldLowercaseEmail()
    {
        // Arrange
        var email = "TEST@EXAMPLE.COM";

        // Act
        var user = User.Create(email, "Test User", "Password123!");

        // Assert
        user.Email.Should().Be("test@example.com");
    }

    #endregion

    #region VerifyEmail Method Tests

    [Fact]
    public void VerifyEmail_OnUnverifiedUser_ShouldSetEmailVerifiedAndClearToken()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");
        user.SetEmailVerificationToken("token123", TimeSpan.FromHours(1));

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerificationToken.Should().BeNull();
        user.EmailVerificationTokenExpiresAt.Should().BeNull();
    }

    [Fact]
    public void VerifyEmail_OnAlreadyVerifiedUser_ShouldStillWork()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");
        user.VerifyEmail(); // Verify first time

        // Act
        user.VerifyEmail(); // Verify again

        // Assert
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerificationToken.Should().BeNull();
        user.EmailVerificationTokenExpiresAt.Should().BeNull();
    }

    #endregion

    #region SetEmailVerificationToken Method Tests

    [Fact]
    public void SetEmailVerificationToken_WithValidData_ShouldSetTokenAndExpiry()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");
        var token = "verification-token-123";
        var validFor = TimeSpan.FromHours(24);

        // Act
        user.SetEmailVerificationToken(token, validFor);

        // Assert
        user.EmailVerificationToken.Should().Be(token);
        user.EmailVerificationTokenExpiresAt.Should().NotBeNull();
        user.EmailVerificationTokenExpiresAt.Should().BeCloseTo(DateTime.UtcNow.Add(validFor), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetEmailVerificationToken_MultipleTimes_ShouldOverridePreviousToken()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");
        
        // Act
        user.SetEmailVerificationToken("first-token", TimeSpan.FromHours(1));
        var firstExpiry = user.EmailVerificationTokenExpiresAt;
        
        Thread.Sleep(10);
        user.SetEmailVerificationToken("second-token", TimeSpan.FromHours(2));

        // Assert
        user.EmailVerificationToken.Should().Be("second-token");
        user.EmailVerificationTokenExpiresAt.Should().BeAfter(firstExpiry!.Value);
    }

    #endregion

    #region UpdateLastLogin Method Tests

    [Fact]
    public void UpdateLastLogin_ShouldSetLastLoginAt()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");

        // Act
        user.UpdateLastLogin();

        // Assert
        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateLastLogin_MultipleTimes_ShouldUpdateToLatestTime()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");
        
        // Act
        user.UpdateLastLogin();
        var firstLoginTime = user.LastLoginAt;

        Thread.Sleep(10);
        user.UpdateLastLogin();

        // Assert
        user.LastLoginAt.Should().BeAfter(firstLoginTime!.Value);
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region VerifyPassword Method Tests

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var user = User.Create("test@example.com", "Test User", password);

        // Act
        var result = user.VerifyPassword(password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "CorrectPassword123!");

        // Act
        var result = user.VerifyPassword("WrongPassword123!");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithDifferentCasing_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "TestPassword123!");

        // Act
        var result = user.VerifyPassword("testpassword123!"); // Different casing

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ChangePassword Method Tests

    [Fact]
    public void ChangePassword_WithValidNewPassword_ShouldUpdatePassword()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "OldPassword123!");
        var originalHashedPassword = user.HashedPassword;
        var newPassword = "NewPassword456!";

        // Act
        user.ChangePassword(newPassword);

        // Assert
        user.HashedPassword.Should().NotBe(originalHashedPassword);
        user.HashedPassword.Should().NotBe(newPassword); // Should be hashed

        // Verify new password works
        user.VerifyPassword(newPassword).Should().BeTrue();
        user.VerifyPassword("OldPassword123!").Should().BeFalse();
    }

    [Fact]
    public void ChangePassword_WithNullPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "OriginalPassword123!");

        // Act & Assert
        var act = () => user.ChangePassword(null!);
        act.Should().Throw<ArgumentException>()
           .WithMessage("New password cannot be empty. (Parameter 'newPassword')");
    }

    [Fact]
    public void ChangePassword_WithEmptyPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "OriginalPassword123!");

        // Act & Assert
        var act = () => user.ChangePassword("");
        act.Should().Throw<ArgumentException>()
           .WithMessage("New password cannot be empty. (Parameter 'newPassword')");
    }

    [Fact]
    public void ChangePassword_WithWhitespacePassword_ShouldThrowArgumentException()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "OriginalPassword123!");

        // Act & Assert
        var act = () => user.ChangePassword("   ");
        act.Should().Throw<ArgumentException>()
           .WithMessage("New password cannot be empty. (Parameter 'newPassword')");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void User_CompleteWorkflow_ShouldWorkCorrectly()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "InitialPassword123!");
        var originalCreatedAt = user.CreatedAt;

        // Act & Assert - Complete user workflow
        
        // 1. Set email verification token
        user.SetEmailVerificationToken("verification-token", TimeSpan.FromHours(24));
        user.EmailVerificationToken.Should().NotBeNull();

        // 2. Verify email
        user.VerifyEmail();
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerificationToken.Should().BeNull();

        // 3. Login (update last login)
        user.UpdateLastLogin();
        user.LastLoginAt.Should().NotBeNull();

        // 4. Change password
        user.ChangePassword("NewPassword456!");
        user.VerifyPassword("NewPassword456!").Should().BeTrue();

        // 5. Login again
        user.UpdateLastLogin();

        // Final assertions
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be("test@example.com");
        user.Name.Should().Be("Test User");
        user.IsEmailVerified.Should().BeTrue();
        user.LastLoginAt.Should().NotBeNull();
        user.CreatedAt.Should().Be(originalCreatedAt);
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_WithEdgeCaseData_ShouldHandleCorrectly()
    {
        // Test with various edge case inputs
        var testCases = new[]
        {
            new { Email = "A@B.C", Name = "A", Password = "Password1!" },
            new { Email = "UPPERCASE@DOMAIN.COM", Name = "User", Password = "Password123!" },
            new { Email = "special+chars@domain.com", Name = "Special & Symbols", Password = "Special!@#Password123" }
        };

        foreach (var testCase in testCases)
        {
            // Act
            var user = User.Create(testCase.Email, testCase.Name, testCase.Password);

            // Assert
            user.Email.Should().Be(testCase.Email.ToLowerInvariant());
            user.Name.Should().Be(testCase.Name);
            user.VerifyPassword(testCase.Password).Should().BeTrue();
            user.IsEmailVerified.Should().BeFalse();
            user.IsActive.Should().BeTrue();
        }
    }

    #endregion
} 