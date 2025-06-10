using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;

namespace SaasTaskManager.Core.Commands.Requests;

public record GetListTasksRequest(string ListId);
public record CreateTaskRequest(
    string Description,
    TaskPriority Priority,
    string ListId
);

public record DeleteTaskRequest(string TaskId);

public record ToggleStatusRequest(string TaskId);

// public record DeleteTaskRequest(string Id, CancellationToken CancellationToken);
//
// public record ToggleTaskStatusRequest(string Id, CancellationToken CancellationToken);