namespace SaasTaskManager.Core.Commands.Responses;

public record CreateUserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string Name { get; init; }
    public bool IsEmailVerified { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record LoginResponse(string Token, UserDto User);

public record UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}

public record ChangePasswordResponse
{
    public string Message { get; init; } = "Password changed successfully";
    public DateTime ChangedAt { get; init; } = DateTime.UtcNow;
}