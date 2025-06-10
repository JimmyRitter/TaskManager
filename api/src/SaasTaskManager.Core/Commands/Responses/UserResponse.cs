namespace SaasTaskManager.Core.Commands.Responses;

public record CreateUserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string Name { get; init; }
    public bool IsEmailVerified { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record LoginResponse(string Token);