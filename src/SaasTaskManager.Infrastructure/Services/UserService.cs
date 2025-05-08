using Microsoft.EntityFrameworkCore;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;
using SaasTaskManager.Core.Interfaces;
using SaasTaskManager.Infrastructure.Data;

namespace SaasTaskManager.Infrastructure.Services;

public class UserService(ApplicationDbContext context) : IUserService
{
    public async Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest user, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user with the same email already exists
            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);

            if (existingUser != null)
            {
                return Result<CreateUserResponse>.Failure("User with this email already exists");
            }

            var newUser = User.Create(user.Email, user.Name, user.Password);
            newUser.SetEmailVerificationToken(Guid.NewGuid().ToString(), TimeSpan.FromDays(1));

            context.Users.Add(newUser);
            await context.SaveChangesAsync(cancellationToken);

            var response = new CreateUserResponse
            {
                Email = newUser.Email,
                Name = newUser.Name,
                Id = newUser.Id
            };

            // return Result<User>.Success(newUser);
            return Result<CreateUserResponse>.Success(response);
        }
        catch (DbUpdateException ex)
        {
            return Result<CreateUserResponse>.Failure($"Failed to create user: {ex.Message}");
        }
    }

    public async Task<Result> VerifyEmailAsync(VerifyEmailRequest command)
    {
        try
        {
            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == command.Email && u.EmailVerificationToken == command.Token);
    
            if (existingUser == null)
            {
                return Result.Failure("Invalid email or token.");
            }
    
            existingUser.VerifyEmail();
    
            context.Users.Update(existingUser);
            await context.SaveChangesAsync();
    
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }
}