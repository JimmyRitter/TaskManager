using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasTaskManager.Api.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Interfaces;

namespace SaasTaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListsController(IListService listService) : ControllerBase
{
    [Authorize]
    [HttpGet("get-all")]
    public async Task<ActionResult<ApiResponse<List<GetUsersListsResponse>>>> GetUsersLists()
    {
        var ownerId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        var result = await listService.GetUsersListsAsync(ownerId);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse<GetUsersListsResponse>.Failure(result.Error));

        return Ok(ApiResponse<List<GetUsersListsResponse>>.Success(result.Value));
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateList([FromBody] CreateListRequest command)
    {
        var ownerId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        var result = await listService.CreateListAsync(command, ownerId);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failure(result.Error));

        return Ok(ApiResponse.Success("List created successfully"));
    }
}