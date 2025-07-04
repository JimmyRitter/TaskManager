using FluentAssertions;
using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Tests.Common;

public class UtilsTests
{
    #region HashPassword Tests (existing coverage extension)

    [Fact]
    public void HashPassword_WithValidEmailAndPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var email = "test@example.com";
        var password = "TestPassword123!";

        // Act
        var hashedPassword = Utils.HashPassword(email, password);

        // Assert
        hashedPassword.Should().NotBeNullOrWhiteSpace();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void HashPassword_WithSameInputs_ShouldReturnConsistentHash()
    {
        // Arrange
        var email = "test@example.com";
        var password = "TestPassword123!";

        // Act
        var hash1 = Utils.HashPassword(email, password);
        var hash2 = Utils.HashPassword(email, password);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void HashPassword_WithNullEmail_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Utils.HashPassword(null!, "password");
        act.Should().Throw<ArgumentException>().WithMessage("Email and password must not be empty.");
    }

    [Fact]
    public void HashPassword_WithEmptyEmail_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Utils.HashPassword("", "password");
        act.Should().Throw<ArgumentException>().WithMessage("Email and password must not be empty.");
    }

    [Fact]
    public void HashPassword_WithWhitespaceEmail_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Utils.HashPassword("   ", "password");
        act.Should().Throw<ArgumentException>().WithMessage("Email and password must not be empty.");
    }

    [Fact]
    public void HashPassword_WithNullPassword_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Utils.HashPassword("test@example.com", null!);
        act.Should().Throw<ArgumentException>().WithMessage("Email and password must not be empty.");
    }

    [Fact]
    public void HashPassword_WithEmptyPassword_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Utils.HashPassword("test@example.com", "");
        act.Should().Throw<ArgumentException>().WithMessage("Email and password must not be empty.");
    }

    [Fact]
    public void HashPassword_WithWhitespacePassword_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Utils.HashPassword("test@example.com", "   ");
        act.Should().Throw<ArgumentException>().WithMessage("Email and password must not be empty.");
    }

    #endregion

    #region ValidatePasswordStrength Tests

    [Fact]
    public void ValidatePasswordStrength_WithValidPassword_ShouldReturnValid()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidatePasswordStrength_WithNullPassword_ShouldReturnInvalid()
    {
        // Act
        var result = Utils.ValidatePasswordStrength(null!);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password is required.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithEmptyPassword_ShouldReturnInvalid()
    {
        // Act
        var result = Utils.ValidatePasswordStrength("");

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password is required.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithWhitespacePassword_ShouldReturnInvalid()
    {
        // Act
        var result = Utils.ValidatePasswordStrength("   ");

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password is required.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithPasswordTooShort_ShouldReturnInvalid()
    {
        // Arrange
        var password = "Test1!"; // 6 characters - too short

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must be at least 8 characters long.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithPasswordTooLong_ShouldReturnInvalid()
    {
        // Arrange
        var password = new string('A', 129) + "1!"; // 131 characters - too long

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must not exceed 128 characters.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithPasswordExactlyMaxLength_ShouldBeValid()
    {
        // Arrange
        var password = new string('A', 125) + "1!a"; // Exactly 128 characters

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain("Password must not exceed 128 characters.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithoutUppercaseLetter_ShouldReturnInvalid()
    {
        // Arrange
        var password = "testpassword123!"; // No uppercase

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithoutLowercaseLetter_ShouldReturnInvalid()
    {
        // Arrange
        var password = "TESTPASSWORD123!"; // No lowercase

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithoutDigit_ShouldReturnInvalid()
    {
        // Arrange
        var password = "TestPassword!"; // No digit

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must contain at least one number.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithoutSpecialCharacter_ShouldReturnInvalid()
    {
        // Arrange
        var password = "TestPassword123"; // No special character

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must contain at least one special character.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithAllSpecialCharacters_ShouldBeValid()
    {
        // Test all accepted special characters
        var specialChars = "!@#$%^&*()_+=[]{};<>|./?,-";
        
        foreach (var specialChar in specialChars)
        {
            // Arrange
            var password = $"TestPassword123{specialChar}";

            // Act
            var result = Utils.ValidatePasswordStrength(password);

            // Assert
            result.IsValid.Should().BeTrue($"Password with special character '{specialChar}' should be valid");
        }
    }

    [Fact]
    public void ValidatePasswordStrength_WithUserInfoContained_ShouldReturnInvalid()
    {
        // Arrange
        var password = "TestJohnDoe123!";
        var userInfo = "johndoe";

        // Act
        var result = Utils.ValidatePasswordStrength(password, userInfo);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must not contain user information.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithPasswordContainedInUserInfo_ShouldReturnInvalid()
    {
        // Arrange
        var password = "Test123!";
        var userInfo = "MyTest123!Password"; // Contains the full password

        // Act
        var result = Utils.ValidatePasswordStrength(password, userInfo);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must not contain user information.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithCaseInsensitiveUserInfo_ShouldReturnInvalid()
    {
        // Arrange
        var password = "TestJOHNDOE123!";
        var userInfo = "johndoe";

        // Act
        var result = Utils.ValidatePasswordStrength(password, userInfo);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must not contain user information.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithNullUserInfo_ShouldNotCheckUserInfo()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var result = Utils.ValidatePasswordStrength(password, null);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain("Password must not contain user information.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithEmptyUserInfo_ShouldNotCheckUserInfo()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var result = Utils.ValidatePasswordStrength(password, "");

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain("Password must not contain user information.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithWhitespaceUserInfo_ShouldNotCheckUserInfo()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var result = Utils.ValidatePasswordStrength(password, "   ");

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain("Password must not contain user information.");
    }

    [Theory]
    [InlineData("password")]
    [InlineData("123456")]
    [InlineData("123456789")]
    [InlineData("qwerty")]
    [InlineData("abc123")]
    [InlineData("password123")]
    [InlineData("admin")]
    [InlineData("letmein")]
    [InlineData("welcome")]
    [InlineData("monkey")]
    [InlineData("dragon")]
    [InlineData("1234567")]
    [InlineData("password1")]
    public void ValidatePasswordStrength_WithCommonPasswords_ShouldReturnInvalid(string commonPassword)
    {
        // Act
        var result = Utils.ValidatePasswordStrength(commonPassword);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password is too common. Please choose a stronger password.");
    }

    [Theory]
    [InlineData("PASSWORD")] // Case insensitive
    [InlineData("Password")]
    [InlineData("QWERTY")]
    [InlineData("Admin")]
    public void ValidatePasswordStrength_WithCommonPasswordsCaseInsensitive_ShouldReturnInvalid(string commonPassword)
    {
        // Act
        var result = Utils.ValidatePasswordStrength(commonPassword);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password is too common. Please choose a stronger password.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithMultipleViolations_ShouldReturnAllErrors()
    {
        // Arrange
        var password = "pass"; // Too short, no uppercase, no digit, no special char

        // Act
        var result = Utils.ValidatePasswordStrength(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Password must be at least 8 characters long.");
        result.Errors.Should().Contain("Password must contain at least one uppercase letter.");
        result.Errors.Should().Contain("Password must contain at least one number.");
        result.Errors.Should().Contain("Password must contain at least one special character.");
        result.Errors.Should().HaveCount(4);
    }

    [Fact]
    public void ValidatePasswordStrength_WithAllViolations_ShouldReturnAllPossibleErrors()
    {
        // Arrange
        var password = "pass"; // Violates multiple rules
        var userInfo = "password"; // Will match common password
        var longPassword = new string('a', 130); // Too long

        // Act
        var shortResult = Utils.ValidatePasswordStrength(password, "pass");
        var longResult = Utils.ValidatePasswordStrength(longPassword);

        // Assert
        shortResult.IsValid.Should().BeFalse();
        shortResult.Errors.Should().HaveCountGreaterThan(4);
        
        longResult.IsValid.Should().BeFalse();
        longResult.Errors.Should().Contain("Password must not exceed 128 characters.");
    }

    [Fact]
    public void ValidatePasswordStrength_WithEdgeCasePasswords_ShouldValidateCorrectly()
    {
        var testCases = new[]
        {
            new { Password = "Aa1!Aa1!", ExpectedValid = true, Description = "Minimum valid password" },
            new { Password = "A1!aaaaa", ExpectedValid = true, Description = "Exactly 8 characters" },
            new { Password = "A1!aaaa", ExpectedValid = false, Description = "7 characters - too short" },
            new { Password = "AA1!aaaa", ExpectedValid = true, Description = "Multiple uppercase" },
            new { Password = "aa1!AAAA", ExpectedValid = true, Description = "Multiple lowercase" },
            new { Password = "Aa123!@#", ExpectedValid = true, Description = "Multiple digits and special chars" }
        };

        foreach (var testCase in testCases)
        {
            // Act
            var result = Utils.ValidatePasswordStrength(testCase.Password);

            // Assert
            result.IsValid.Should().Be(testCase.ExpectedValid, $"Password '{testCase.Password}' - {testCase.Description}");
        }
    }

    #endregion

    #region PasswordValidationResult Tests

    [Fact]
    public void PasswordValidationResult_WithValidPassword_ShouldHaveCorrectProperties()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        var result = new PasswordValidationResult(true, errors);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PasswordValidationResult_WithErrors_ShouldHaveCorrectProperties()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2" };

        // Act
        var result = new PasswordValidationResult(false, errors);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain("Error 1");
        result.Errors.Should().Contain("Error 2");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Utils_CompletePasswordValidationFlow_ShouldWorkCorrectly()
    {
        // Test a complete flow: validate password, then hash it
        // Arrange
        var email = "test@example.com";
        var password = "TestPassword123!";

        // Act
        var validationResult = Utils.ValidatePasswordStrength(password, email);
        
        string? hashedPassword = null;
        if (validationResult.IsValid)
        {
            hashedPassword = Utils.HashPassword(email, password);
        }

        // Assert
        validationResult.IsValid.Should().BeTrue();
        hashedPassword.Should().NotBeNull();
        hashedPassword.Should().NotBe(password);
    }

    [Fact]
    public void Utils_PasswordValidationWithUserEmail_ShouldPreventEmailInPassword()
    {
        // Arrange
        var email = "john.doe@example.com";
        var password = "john.doe@example.com123!"; // Contains full email

        // Act
        var validationResult = Utils.ValidatePasswordStrength(password, email);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain("Password must not contain user information.");
    }

    #endregion
} 