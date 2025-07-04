using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Entities;
using SaasTaskManager.Infrastructure.Data;
using SaasTaskManager.Infrastructure.Services;
using Task = System.Threading.Tasks.Task;

namespace SaasTaskManager.Tests.Services;

public class UserServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        
        // Setup configuration mock
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("your-super-secret-key-with-at-least-32-characters");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        _configurationMock.Setup(c => c["Jwt:ExpirationInMinutes"]).Returns("60");

        _userService = new UserService(_context, _configurationMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateUserRequest("test@example.com", "Test User", "Password123!");

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("test@example.com");
        result.Value!.Name.Should().Be("Test User");
        
        // Verify user was saved to database
        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ShouldReturnFailure()
    {
        // Arrange
        var existingUser = User.Create("test@example.com", "Existing User", "Password123!");
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new CreateUserRequest("test@example.com", "Test User", "Password123!");

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("User with this email already exists");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokenAndUser()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginRequest("test@example.com", "Password123!");

        // Act
        var result = await _userService.LoginAsync(loginRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Token.Should().NotBeNullOrEmpty();
        result.Value!.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "Password123!");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginRequest("test@example.com", "WrongPassword!");

        // Act
        var result = await _userService.LoginAsync(loginRequest);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidCurrentPassword_ShouldReturnSuccess()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "OldPassword123!");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new ChangePasswordRequest 
        { 
            CurrentPassword = "OldPassword123!", 
            NewPassword = "NewPassword123!" 
        };

        // Act
        var result = await _userService.ChangePasswordAsync(user.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify password was actually changed
        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.VerifyPassword("NewPassword123!").Should().BeTrue();
        updatedUser!.VerifyPassword("OldPassword123!").Should().BeFalse();
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User", "OldPassword123!");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new ChangePasswordRequest 
        { 
            CurrentPassword = "WrongPassword!", 
            NewPassword = "NewPassword123!" 
        };

        // Act
        var result = await _userService.ChangePasswordAsync(user.Id, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Current password is incorrect");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 