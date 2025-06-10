using SaasTaskManager.Core.Common;

namespace SaasTaskManager.Core.Entities;

public class ListShare
{
    public Guid Id { get; private set; }
    public Guid ListId { get; private set; }
    public Guid SharedWithUserId { get; private set; }
    public SharePermission Permission { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ListShare()
    {
    }

    public static ListShare Create(Guid listId, Guid sharedWithUserId, SharePermission permission)
    {
        return new ListShare
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            SharedWithUserId = sharedWithUserId,
            Permission = permission,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdatePermission(SharePermission newPermission)
    {
        Permission = newPermission;
        UpdatedAt = DateTime.UtcNow;
    }
}