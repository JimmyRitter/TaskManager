using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Commands.Responses;

namespace SaasTaskManager.Core.Interfaces;

public interface IListService
{
    Task<Result<List<GetUsersListsResponse>>> GetUsersListsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> CreateListAsync(CreateListRequest command, Guid ownerId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteListAsync(DeleteListRequest command,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateListAsync(UpdateListRequest command,
        CancellationToken cancellationToken = default);
}

// please generate the methods for the ListService. There need to have a CreateList (where it will create an empty list, need to receive a name, a description and a category). Also need to have a Delete List (self explanatory), and an Update  List, where it can update the name, description, or category, and also update who the list is being shared with. 