using Microsoft.AspNetCore.Mvc;
using SaasTaskManager.Api.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Interfaces;

namespace SaasTaskManager.Api.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("test")]
    public async Task<string> GetTest()
    {
        return await Task.FromResult("it works");
    }
    

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateUserResponse>>> CreateUser([FromBody] CreateUserRequest command)
    {
        var result = await _userService.CreateUserAsync(command);
        
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<CreateUserResponse>.Failure(result.Error));

        return Ok(ApiResponse<CreateUserResponse>.Success(result.Value, "User created successfully"));
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<ApiResponse>> VerifyEmail([FromBody] VerifyEmailRequest command)
    {
        var result = await _userService.VerifyEmailAsync(command);
        
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failure(result.Error));

        return Ok(ApiResponse.Success("Email verified successfully"));
    }
}