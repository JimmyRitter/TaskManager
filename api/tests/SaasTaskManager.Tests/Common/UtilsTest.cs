using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Tests.Common;

public class UtilsTest
{
    [Fact]
    public void HashPassword_ReturnsConsistentHash()
    {
        // Arrange
        string email = "test@example.com";
        string password = "password123";

        // Act
        string hash1 = Utils.HashPassword(email, password);
        string hash2 = Utils.HashPassword(email, password);

        // Assert
        Assert.Equal(hash1, hash2); // Consistency check
        Assert.False(string.IsNullOrWhiteSpace(hash1)); // Not empty
    }

    [Fact]
    public void HashPassword_EmptyInput_ThrowsException()
    {
        // Arrange
        string email = "";
        string password = "password123";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Utils.HashPassword(email, password));
    }

    [Fact]
    public void HashPassword_SpecificInput_ReturnsExpectedHash()
    {
        // Arrange
        string email = "email@email.com";
        string password = "password";
        string expectedHash = "z230fb+j1Ds6tu3FbZuZjOgTj6nnEnfGO4B5lfMCr34=";

        // Act
        string actualHash = Utils.HashPassword(email, password);

        // Assert
        Assert.Equal(expectedHash, actualHash);
    }
}
