using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;

namespace SaasTaskManager.Core.Commands.Requests;

public record CreateListRequest(
    string Name,
    string Description,
    ListCategory Category,
    Guid OwnerId,
    CancellationToken CancellationToken);

public record DeleteListRequest(string Id, CancellationToken CancellationToken);

public record UpdateListRequest(
    string Name,
    string Description,
    ListCategory Category,
    List<ListShare> ListShares,
    string Id,
    CancellationToken CancellationToken);