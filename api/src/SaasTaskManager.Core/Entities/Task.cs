using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Core.Entities;

public class Task
{
    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public TaskPriority Priority { get; private set; }
    public bool IsCompleted { get; private set; }
    public Guid ListId { get; private set; }
    public int Order { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    

    // private readonly List<TaskStatusChange> _statusHistory = new();
    // public IReadOnlyCollection<TaskStatusChange> StatusHistory => _statusHistory.AsReadOnly();

    private Task() { }

    public static Task Create(string description, TaskPriority priority, Guid listId, int order = 0)
    {
        return new Task
        {
            Id = Guid.NewGuid(),
            Description = description,
            Priority = priority,
            ListId = listId,
            Order = order,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void ToggleStatus()
    {
        IsCompleted = !IsCompleted;
        UpdatedAt = DateTime.UtcNow;
        
        // var statusChange = TaskStatusChange.Create(Id, userId, IsCompleted);
        // _statusHistory.Add(statusChange);
    }
    
    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Recreate()
    {
        if (!DeletedAt.HasValue) return;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOrder(int newOrder)
    {
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTask(string? description = null, TaskPriority? priority = null, DateTime? dueDate = null)
    {
        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
            
        if (priority.HasValue)
            Priority = priority.Value;
            
        if (dueDate.HasValue)
            DueDate = dueDate.Value;
            
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsPastDueDate => DueDate.HasValue && DueDate.Value < DateTime.UtcNow;
    
}