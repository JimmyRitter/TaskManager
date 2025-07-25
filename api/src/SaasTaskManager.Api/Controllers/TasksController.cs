﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasTaskManager.Api.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Interfaces;

namespace SaasTaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [Authorize]
    [HttpGet("get-list-tasks")]
    public async Task<ActionResult<ApiResponse<List<GetListTasksResponse>>>> GetTasks([FromBody] GetListTasksRequest command, CancellationToken cancellationToken = default)
    {
        // var ownerId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        
        var result = await taskService.GetListTasksAsync(new Guid(command.ListId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse<GetListTasksResponse>.Failure(result.Error));

        return Ok(ApiResponse<List<GetListTasksResponse>>.Success(result.Value));
    }
    
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse>> CreateTask([FromBody] CreateTaskRequest command, CancellationToken cancellationToken = default)
    {
        var ownerId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        var result = await taskService.CreateTaskAsync(command, ownerId, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failure(result.Error));

        return Ok(ApiResponse.Success("Task created successfully"));
    }
    
    [Authorize]
    [HttpDelete("delete")]
    public async Task<ActionResult<ApiResponse>> DeleteTask([FromBody] DeleteTaskRequest command, CancellationToken cancellationToken = default)
    {
        var result = await taskService.DeleteTaskAsync(command.TaskId.ToString(), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failure(result.Error));

        return Ok(ApiResponse.Success("Task deleted successfully"));
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<ActionResult<ApiResponse<GetListTasksResponse>>> UpdateTask([FromBody] UpdateTaskRequest command, CancellationToken cancellationToken = default)
    {
        var result = await taskService.UpdateTaskAsync(command);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<GetListTasksResponse>.Failure(result.Error));

        return Ok(ApiResponse<GetListTasksResponse>.Success(result.Value));
    }
    
    [Authorize]
    [HttpPut("toggle")]
    public async Task<ActionResult<ApiResponse>> ToggleTaskStatus([FromBody] ToggleTaskStatusRequest command, CancellationToken cancellationToken = default)
    {
        var result = await taskService.ToggleTaskStatusAsync(command.TaskId.ToString(), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failure(result.Error));

        return Ok(ApiResponse.Success("Task toggled successfully"));
    }

    [Authorize]
    [HttpPut("update-order")]
    public async Task<ActionResult<ApiResponse>> UpdateTaskOrder([FromBody] UpdateTaskOrderRequest command, CancellationToken cancellationToken = default)
    {
        var result = await taskService.UpdateTaskOrderAsync(command, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failure(result.Error));

        return Ok(ApiResponse.Success("Task order updated successfully"));
    }
}