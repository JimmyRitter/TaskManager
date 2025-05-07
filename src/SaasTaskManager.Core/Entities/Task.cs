using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Core.Entities;

public class Task
{
    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public TaskPriority Priority { get; private set; }
    public bool IsCompleted { get; private set; }
    public Guid ListId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<TaskStatusChange> _statusHistory = new();
    public IReadOnlyCollection<TaskStatusChange> StatusHistory => _statusHistory.AsReadOnly();

    private Task() { }

    public static Task Create(string description, TaskPriority priority, Guid listId)
    {
        return new Task
        {
            Id = Guid.NewGuid(),
            Description = description,
            Priority = priority,
            ListId = listId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void ToggleStatus(Guid userId)
    {
        IsCompleted = !IsCompleted;
        UpdatedAt = DateTime.UtcNow;
        
        var statusChange = TaskStatusChange.Create(Id, userId, IsCompleted);
        _statusHistory.Add(statusChange);
    }
}