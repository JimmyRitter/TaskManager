﻿using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;
using SaasTaskManager.Core.Interfaces;

namespace SaasTaskManager.Core.Commands.Responses;

public record GetListTasksResponse(
    Guid Id,
    string Description,
    TaskPriority Priority,
    bool IsCompleted,
    Guid ListId,
    int Order,
    DateTime? DueDate,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    DateTime CreatedAt
);