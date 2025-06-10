using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;
using SaasTaskManager.Core.Interfaces;
using SaasTaskManager.Infrastructure.Data;

namespace SaasTaskManager.Infrastructure.Services;

public class UserService(ApplicationDbContext context, IConfiguration configuration) : IUserService
{
    public async Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest user,
        CancellationToken cancellationToken = default)
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

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var password = Utils.HashPassword(request.Email, request.Password);
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.HashedPassword == password, cancellationToken);

            if (user == null)
            {
                return Result<LoginResponse>.Failure("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            var response = new LoginResponse(token);
            return Result<LoginResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<LoginResponse>.Failure($"Login failed: {ex.Message}");
        }
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", user.Id.ToString()), // subject (user id)
            new Claim("email", user.Email), // email
            new Claim("name", user.Name), // name
            new Claim("jti", Guid.NewGuid().ToString()), // unique token id
            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64) // issued at
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Jwt:ExpirationInMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}