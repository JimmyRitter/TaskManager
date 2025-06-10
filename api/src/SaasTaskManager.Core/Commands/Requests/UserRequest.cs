namespace SaasTaskManager.Core.Commands.Requests;

// public record CreateUserRequest
// {
//     public string Email { get; init; }
//     public string Name { get; init; }
//     public string Password { get; init; }
// }

// public record VerifyEmailRequest
// {
//     public string Email { get; init; }
//     public string Token { get; init; }
// }

public record CreateUserRequest(string Email, string Name, string Password);

public record VerifyEmailRequest(string Email, string Token);

public record LoginRequest(string Email, string Password);