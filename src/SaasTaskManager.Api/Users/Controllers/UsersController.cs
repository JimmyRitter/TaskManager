using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasTaskManager.Api.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Interfaces;

namespace SaasTaskManager.Api.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateUserResponse>>> CreateUser([FromBody] CreateUserRequest command)
    {
        var result = await userService.CreateUserAsync(command);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse<CreateUserResponse>.Failure(result.Error));

        return Ok(ApiResponse<CreateUserResponse>.Success(result.Value, "User created successfully"));
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<ApiResponse>> VerifyEmail([FromBody] VerifyEmailRequest command)
    {
        var result = await userService.VerifyEmailAsync(command);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failure(result.Error));

        return Ok(ApiResponse.Success("Email verified successfully"));
    }


    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest command)
    {
        var result = await userService.LoginAsync(command);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse<LoginResponse>.Failure(result.Error));

        return Ok(ApiResponse<LoginResponse>.Success(result.Value, "Login successful"));
    }
    
    [Authorize]
    [HttpGet("protected")]
    public async Task<ActionResult<string>> ProtectedEndpoint()
    {
        return Ok("This is a protected endpoint");
    }
}