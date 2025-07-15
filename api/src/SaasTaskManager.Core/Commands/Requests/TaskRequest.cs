using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;

namespace SaasTaskManager.Core.Commands.Requests;

public record GetListTasksRequest(string ListId);
public record CreateTaskRequest(
    string Description,
    TaskPriority Priority,
    string ListId,
    int? Order = null
);

public record UpdateTaskRequest(Guid TaskId, string? Description = null, TaskPriority? Priority = null, DateTime? DueDate = null);

public record DeleteTaskRequest(Guid TaskId);

public record ToggleTaskStatusRequest(Guid TaskId);

public record UpdateTaskOrderRequest(string TaskId, int NewOrder);
