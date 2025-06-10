using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Core.Entities;

public class List
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ListCategory Category { get; private set; }
    public Guid OwnerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<Task> _tasks = new();
    private readonly List<ListShare> _shares = new();

    public IReadOnlyCollection<Task> Tasks => _tasks.AsReadOnly();
    public IReadOnlyCollection<ListShare> Shares => _shares.AsReadOnly();

    private List()
    {
    }

    public static List Create(string name, string description, ListCategory category, Guid ownerId)
    {
        return new List
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Category = category,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddTask(Task task)
    {
        _tasks.Add(task);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ShareWith(User user, SharePermission permission)
    {
        var share = ListShare.Create(this.Id, user.Id, permission);
        _shares.Add(share);
        UpdatedAt = DateTime.UtcNow;
    }
}