namespace SaasTaskManager.Core.Entities;

public class TaskStatusChange
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public Guid ChangedByUserId { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TaskStatusChange() { }

    public static TaskStatusChange Create(Guid taskId, Guid userId, bool isCompleted)
    {
        return new TaskStatusChange
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            ChangedByUserId = userId,
            IsCompleted = isCompleted,
            CreatedAt = DateTime.UtcNow
        };
    }
}