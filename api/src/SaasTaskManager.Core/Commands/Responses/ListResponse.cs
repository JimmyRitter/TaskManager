using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;
using SaasTaskManager.Core.Interfaces;

namespace SaasTaskManager.Core.Commands.Responses;

public record GetUsersListsResponse(
    Guid Id,
    string Name,
    string Description,
    ListCategory Category,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateListResponse(
    Guid Id,
    string Name,
    string Description
);

// public record CreateListResponse(
//     string Name,
//     string Description,
//     ListCategory Category,
//     CancellationToken CancellationToken);
//
// public record UpdateListResponse(
//     string Name,
//     string Description,
//     ListCategory Category,
//     List<ListShare> ListShares,
//     string Id,
//     CancellationToken CancellationToken);