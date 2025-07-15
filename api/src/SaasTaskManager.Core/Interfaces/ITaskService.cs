using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Core.Interfaces;

public interface ITaskService
{
    Task<Result<List<GetListTasksResponse>>> GetListTasksAsync(Guid listId, CancellationToken cancellationToken);
    
    Task<Result> CreateTaskAsync(CreateTaskRequest request, Guid ownerId, CancellationToken cancellationToken);
    Task<Result> DeleteTaskAsync(string taskId, CancellationToken cancellationToken);
    Task<Result> ToggleTaskStatusAsync(string taskId, CancellationToken cancellationToken);
    Task<Result> UpdateTaskOrderAsync(UpdateTaskOrderRequest request, CancellationToken cancellationToken);
    Task<Result<GetListTasksResponse>> UpdateTaskAsync(UpdateTaskRequest request);
}