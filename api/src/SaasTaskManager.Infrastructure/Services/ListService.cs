﻿using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Interfaces;
using SaasTaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SaasTaskManager.Core.Commands.Responses;
using SaasTaskManager.Core.Entities;

public class ListService(ApplicationDbContext dbContext) : IListService
{
    public async Task<Result<List<GetUsersListsResponse>>> GetUsersListsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var lists = await dbContext.Lists.Where((x) => x.OwnerId == userId).ToListAsync(cancellationToken);
        if (lists.Count == 0)
        {
            return Result<List<GetUsersListsResponse>>.Failure("No lists found");
        }

        var returnLists = new List<GetUsersListsResponse>();

        returnLists.AddRange(lists.Select(i =>
            new GetUsersListsResponse(i.Id, i.Name, i.Description, i.Category, i.CreatedAt, i.UpdatedAt))
        );


        return Result<List<GetUsersListsResponse>>.Success(returnLists);
    }

    public async Task<Result> CreateListAsync(CreateListRequest command, Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var list = List.Create(command.Name, command.Description, command.Category, ownerId);

            await dbContext.Lists.AddAsync(list, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create list: {ex.Message}");
        }
    }

    public async Task<Result> DeleteListAsync(DeleteListRequest command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await dbContext.Lists
                .FirstOrDefaultAsync(l => l.Id == Guid.Parse(command.Id), cancellationToken);

            if (list == null)
            {
                return Result.Failure("List not found");
            }

            dbContext.Lists.Remove(list);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete list: {ex.Message}");
        }
    }

    public async Task<Result> UpdateListAsync(UpdateListRequest command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // var list = await dbContext.Lists
            //     .Include(l => l.ListShares)
            //     .FirstOrDefaultAsync(l => l.Id == Guid.Parse(command.Id), cancellationToken);
            //
            // if (list == null)
            // {
            //     return Result.Failure("List not found");
            // }
            //
            // // Update basic properties
            // list.Name = command.Name;
            // list.Description = command.Description;
            // list.Category = command.Category;
            // list.UpdatedAt = DateTime.UtcNow;
            //
            // // Update list shares
            // dbContext.ListShares.RemoveRange(list.ListShares);
            // if (command.ListShares != null && command.ListShares.Any())
            // {
            //     await dbContext.ListShares.AddRangeAsync(command.ListShares, cancellationToken);
            // }
            //
            // await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update list: {ex.Message}");
        }
    }
}