using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Interfaces;
using SaasTaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Entities;
using Task = SaasTaskManager.Core.Entities.Task;

namespace SaasTaskManager.Infrastructure.Services;

public class TaskService(ApplicationDbContext dbContext) : ITaskService
{
    public async Task<Result<List<GetListTasksResponse>>> GetListTasksAsync(Guid listId,
        CancellationToken cancellationToken)
    {
        var tasks = await dbContext.Tasks.Where(t => t.ListId == listId)
            .OrderBy(t => t.DeletedAt.HasValue ? 1 : 0) // Active tasks first (DeletedAt == null)
            .ThenBy(t => t.Order) // Then by order for active tasks
            .ThenByDescending(t => t.DeletedAt) // Then by deletion date for deleted tasks
            .ToListAsync(cancellationToken);

        var returnTasks = new List<GetListTasksResponse>();

        returnTasks.AddRange(tasks.Select(t =>
            new GetListTasksResponse(t.Id, t.Description, t.Priority, t.IsCompleted, t.ListId, t.Order, t.DueDate, t.UpdatedAt,
                t.DeletedAt, t.CreatedAt))
        );

        return Result<List<GetListTasksResponse>>.Success(returnTasks);
    }

    public async Task<Result> CreateTaskAsync(CreateTaskRequest command, Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var listIdGuid = Guid.Parse(command.ListId);
            var existingList =
                dbContext.Lists.FirstOrDefault(l => l.Id == listIdGuid && l.OwnerId == ownerId);

            if (existingList == null)
            {
                return Result.Failure("The given list doesn't exist or doesn't belong to the authenticated user.");
            }

            // Use provided order or calculate the next order value for the new task
            var taskOrder = command.Order;
            if (!taskOrder.HasValue)
            {
                var maxOrder = await dbContext.Tasks
                    .Where(t => t.ListId == listIdGuid && t.DeletedAt == null)
                    .MaxAsync(t => (int?)t.Order, cancellationToken) ?? -1;
                
                taskOrder = maxOrder + 1;
            }

            var task = Task.Create(command.Description, command.Priority, listIdGuid, taskOrder.Value);
            await dbContext.Tasks.AddAsync(task, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create task: {ex.Message}");
        }
    }

    public async Task<Result> DeleteTaskAsync(string taskId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var task = await dbContext.Tasks
                .FirstOrDefaultAsync(l => l.Id == Guid.Parse(taskId), cancellationToken);

            if (task == null)
            {
                return Result.Failure("Task not found");
            }
            
            task.Delete();

            dbContext.Tasks.Update(task);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete list: {ex.Message}");
        }
    }

    public async Task<Result> ToggleTaskStatusAsync(string taskId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var task = await dbContext.Tasks
                .FirstOrDefaultAsync(l => l.Id == Guid.Parse(taskId), cancellationToken);

            if (task == null)
            {
                return Result.Failure("Task not found");
            }

            task.ToggleStatus();

            dbContext.Tasks.Update(task);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to toggle task status: {ex.Message}");
        }
    }
}