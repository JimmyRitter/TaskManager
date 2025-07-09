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

public record DeleteTaskRequest(string TaskId);

public record ToggleStatusRequest(string TaskId);

public record UpdateTaskOrderRequest(string TaskId, int NewOrder);
