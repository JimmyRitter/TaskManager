using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;

namespace SaasTaskManager.Core.Interfaces;

public interface IUserService
{
    Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest command,
        CancellationToken cancellationToken = default);

    Task<Result> VerifyEmailAsync(VerifyEmailRequest command);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}