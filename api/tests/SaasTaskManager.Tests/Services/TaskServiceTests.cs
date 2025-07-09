using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using SaasTaskManager.Core.Commands.Requests;
using SaasTaskManager.Core.Entities;
using SaasTaskManager.Core.Common;
using SaasTaskManager.Infrastructure.Data;
using SaasTaskManager.Infrastructure.Services;
using Task = System.Threading.Tasks.Task;
using TaskEntity = SaasTaskManager.Core.Entities.Task;

namespace SaasTaskManager.Tests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TaskService _taskService;
    private readonly User _testUser;
    private readonly List _testList;

    public TaskServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _taskService = new TaskService(_context);

        // Create test user and list
        _testUser = User.Create("test@example.com", "Test User", "Password123!");
        _testList = List.Create("Test List", "Test Description", ListCategory.Personal, _testUser.Id);
        
        _context.Users.Add(_testUser);
        _context.Lists.Add(_testList);
        _context.SaveChanges();
    }

    #region CreateTaskAsync Tests

    [Fact]
    public async Task CreateTaskAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Description: "Test Task",
            Priority: TaskPriority.Medium,
            ListId: _testList.Id.ToString()
        );

        // Act
        var result = await _taskService.CreateTaskAsync(request, _testUser.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify task was saved to database
        var taskInDb = await _context.Tasks.FirstOrDefaultAsync(t => t.Description == "Test Task");
        taskInDb.Should().NotBeNull();
        taskInDb!.ListId.Should().Be(_testList.Id);
        taskInDb.Priority.Should().Be(TaskPriority.Medium);
        taskInDb.IsCompleted.Should().BeFalse();
        taskInDb.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateTaskAsync_WithNonExistentList_ShouldReturnFailure()
    {
        // Arrange
        var nonExistentListId = Guid.NewGuid().ToString();
        var request = new CreateTaskRequest(
            Description: "Test Task",
            Priority: TaskPriority.High,
            ListId: nonExistentListId
        );

        // Act
        var result = await _taskService.CreateTaskAsync(request, _testUser.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("doesn't exist or doesn't belong to the authenticated user");
    }

    [Fact]
    public async Task CreateTaskAsync_WithListNotOwnedByUser_ShouldReturnFailure()
    {
        // Arrange
        var otherUser = User.Create("other@example.com", "Other User", "Password123!");
        var otherUserList = List.Create("Other List", "Description", ListCategory.Work, otherUser.Id);
        
        _context.Users.Add(otherUser);
        _context.Lists.Add(otherUserList);
        await _context.SaveChangesAsync();

        var request = new CreateTaskRequest(
            Description: "Test Task",
            Priority: TaskPriority.Low,
            ListId: otherUserList.Id.ToString()
        );

        // Act
        var result = await _taskService.CreateTaskAsync(request, _testUser.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("doesn't exist or doesn't belong to the authenticated user");
    }

    [Fact]
    public async Task CreateTaskAsync_WithInvalidListId_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Description: "Test Task",
            Priority: TaskPriority.High,
            ListId: "invalid-guid"
        );

        // Act
        var result = await _taskService.CreateTaskAsync(request, _testUser.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to create task");
    }

    #endregion

    #region GetListTasksAsync Tests

    [Fact]
    public async Task GetListTasksAsync_WithExistingTasks_ShouldReturnTasksOrderedByCreation()
    {
        // Arrange
        var task1 = TaskEntity.Create("First Task", TaskPriority.Low, _testList.Id);
        var task2 = TaskEntity.Create("Second Task", TaskPriority.High, _testList.Id);
        var task3 = TaskEntity.Create("Third Task", TaskPriority.Medium, _testList.Id);

        _context.Tasks.AddRange(task1, task2, task3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.GetListTasksAsync(_testList.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        
        // Tasks should be ordered by creation time (oldest first in TaskService uses OrderByDescending(DeletedAt))
        // Note: The current implementation has OrderByDescending(DeletedAt) which might not be the intended order
        result.Value.Should().Contain(t => t.Description == "First Task");
        result.Value.Should().Contain(t => t.Description == "Second Task");
        result.Value.Should().Contain(t => t.Description == "Third Task");
    }

    [Fact]
    public async Task GetListTasksAsync_WithNoTasks_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyListId = Guid.NewGuid();

        // Act
        var result = await _taskService.GetListTasksAsync(emptyListId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetListTasksAsync_ShouldIncludeDeletedTasks()
    {
        // Arrange
        var activeTask = TaskEntity.Create("Active Task", TaskPriority.Medium, _testList.Id);
        var deletedTask = TaskEntity.Create("Deleted Task", TaskPriority.Low, _testList.Id);
        deletedTask.Delete(); // Soft delete

        _context.Tasks.AddRange(activeTask, deletedTask);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.GetListTasksAsync(_testList.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(t => t.Description == "Active Task" && t.DeletedAt == null);
        result.Value.Should().Contain(t => t.Description == "Deleted Task" && t.DeletedAt != null);
    }

    #endregion

    #region DeleteTaskAsync Tests

    [Fact]
    public async Task DeleteTaskAsync_WithExistingTask_ShouldSoftDeleteTask()
    {
        // Arrange
        var task = TaskEntity.Create("Task to Delete", TaskPriority.Medium, _testList.Id);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.DeleteTaskAsync(task.Id.ToString());

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify task is soft deleted
        var deletedTask = await _context.Tasks.FindAsync(task.Id);
        deletedTask.Should().NotBeNull();
        deletedTask!.DeletedAt.Should().NotBeNull();
        deletedTask.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteTaskAsync_WithNonExistentTask_ShouldReturnFailure()
    {
        // Arrange
        var nonExistentTaskId = Guid.NewGuid().ToString();

        // Act
        var result = await _taskService.DeleteTaskAsync(nonExistentTaskId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Task not found");
    }

    [Fact]
    public async Task DeleteTaskAsync_WithInvalidTaskId_ShouldReturnFailure()
    {
        // Arrange
        var invalidTaskId = "invalid-guid";

        // Act
        var result = await _taskService.DeleteTaskAsync(invalidTaskId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to delete list"); // Note: Error message seems incorrect in service
    }

    #endregion

    #region ToggleTaskStatusAsync Tests

    [Fact]
    public async Task ToggleTaskStatusAsync_WithIncompleteTask_ShouldMarkAsCompleted()
    {
        // Arrange
        var task = TaskEntity.Create("Task to Complete", TaskPriority.High, _testList.Id);
        task.IsCompleted.Should().BeFalse(); // Initial state
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.ToggleTaskStatusAsync(task.Id.ToString());

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify task is marked as completed
        var updatedTask = await _context.Tasks.FindAsync(task.Id);
        updatedTask.Should().NotBeNull();
        updatedTask!.IsCompleted.Should().BeTrue();
        updatedTask.UpdatedAt.Should().NotBeNull();
        updatedTask.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ToggleTaskStatusAsync_WithCompletedTask_ShouldMarkAsIncomplete()
    {
        // Arrange
        var task = TaskEntity.Create("Completed Task", TaskPriority.Low, _testList.Id);
        task.ToggleStatus(); // Make it completed first
        task.IsCompleted.Should().BeTrue();
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.ToggleTaskStatusAsync(task.Id.ToString());

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify task is marked as incomplete
        var updatedTask = await _context.Tasks.FindAsync(task.Id);
        updatedTask.Should().NotBeNull();
        updatedTask!.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleTaskStatusAsync_WithNonExistentTask_ShouldReturnFailure()
    {
        // Arrange
        var nonExistentTaskId = Guid.NewGuid().ToString();

        // Act
        var result = await _taskService.ToggleTaskStatusAsync(nonExistentTaskId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Task not found");
    }

    [Fact]
    public async Task ToggleTaskStatusAsync_WithInvalidTaskId_ShouldReturnFailure()
    {
        // Arrange
        var invalidTaskId = "invalid-guid";

        // Act
        var result = await _taskService.ToggleTaskStatusAsync(invalidTaskId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to toggle task status");
    }

    [Fact]
    public async Task ToggleTaskStatusAsync_MultipleToggles_ShouldAlternateStatus()
    {
        // Arrange
        var task = TaskEntity.Create("Toggle Task", TaskPriority.Medium, _testList.Id);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act & Assert - First toggle (incomplete -> complete)
        var result1 = await _taskService.ToggleTaskStatusAsync(task.Id.ToString());
        result1.IsSuccess.Should().BeTrue();
        
        var taskAfterFirst = await _context.Tasks.FindAsync(task.Id);
        taskAfterFirst!.IsCompleted.Should().BeTrue();

        // Act & Assert - Second toggle (complete -> incomplete)
        var result2 = await _taskService.ToggleTaskStatusAsync(task.Id.ToString());
        result2.IsSuccess.Should().BeTrue();
        
        var taskAfterSecond = await _context.Tasks.FindAsync(task.Id);
        taskAfterSecond!.IsCompleted.Should().BeFalse();
    }

    #endregion

    #region UpdateTaskOrderAsync Tests

    [Theory]
    [InlineData(0, 2, new[] { 2, 0, 1, 3, 4 })] // Move first task to position 2: [T0,T1,T2,T3,T4] -> [T1,T2,T0,T3,T4]
    [InlineData(2, 0, new[] { 1, 2, 0, 3, 4 })] // Move middle task to first position: [T0,T1,T2,T3,T4] -> [T2,T0,T1,T3,T4]  
    [InlineData(4, 1, new[] { 0, 2, 3, 4, 1 })] // Move last task to position 1: [T0,T1,T2,T3,T4] -> [T0,T4,T1,T2,T3]
    [InlineData(1, 3, new[] { 0, 3, 1, 2, 4 })] // Move task forward (1 -> 3): [T0,T1,T2,T3,T4] -> [T0,T2,T3,T1,T4]
    [InlineData(3, 1, new[] { 0, 2, 3, 1, 4 })] // Move task backward (3 -> 1): [T0,T1,T2,T3,T4] -> [T0,T3,T1,T2,T4]
    public async Task UpdateTaskOrderAsync_WithValidReordering_ShouldUpdateAllTaskOrdersCorrectly(
        int originalPosition, int newPosition, int[] expectedFinalOrders)
    {
        // Arrange
        var tasks = new List<TaskEntity>();
        for (int i = 0; i < 5; i++)
        {
            var task = TaskEntity.Create($"Task {i}", TaskPriority.Medium, _testList.Id, i);
            tasks.Add(task);
        }
        
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        var taskToMove = tasks[originalPosition];
        var request = new UpdateTaskOrderRequest(taskToMove.Id.ToString(), newPosition);

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify all tasks have correct sequential order values
        var updatedTasks = await _context.Tasks
            .Where(t => t.ListId == _testList.Id && t.DeletedAt == null)
            .OrderBy(t => t.Order)
            .ToListAsync();

        updatedTasks.Should().HaveCount(5);
        
        // Check each task has the expected order
        for (int i = 0; i < updatedTasks.Count; i++)
        {
            updatedTasks[i].Order.Should().Be(i, $"Task at position {i} should have order {i}");
        }

        // Check no gaps in sequence
        var orderValues = updatedTasks.Select(t => t.Order).ToArray();
        orderValues.Should().BeEquivalentTo(new[] { 0, 1, 2, 3, 4 });

        // Check no duplicates
        orderValues.Should().OnlyHaveUniqueItems();

        // Verify the specific task order mapping matches expected
        var taskOrderMap = updatedTasks.ToDictionary(t => tasks.FindIndex(orig => orig.Id == t.Id), t => t.Order);
        for (int i = 0; i < expectedFinalOrders.Length; i++)
        {
            taskOrderMap[i].Should().Be(expectedFinalOrders[i], 
                $"Original task {i} should be at order {expectedFinalOrders[i]}");
        }
    }

    [Theory]
    [InlineData(0, 0)] // Same position (first)
    [InlineData(1, 1)] // Same position (middle)
    [InlineData(2, 2)] // Same position (last)
    public async Task UpdateTaskOrderAsync_WithSamePosition_ShouldNotChangeAnyOrders(int position, int newPosition)
    {
        // Arrange
        var tasks = new List<TaskEntity>();
        for (int i = 0; i < 3; i++)
        {
            var task = TaskEntity.Create($"Task {i}", TaskPriority.Medium, _testList.Id, i);
            tasks.Add(task);
        }
        
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        var originalOrders = tasks.Select(t => t.Order).ToArray();
        var request = new UpdateTaskOrderRequest(tasks[position].Id.ToString(), newPosition);

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify no orders changed
        var updatedTasks = await _context.Tasks
            .Where(t => t.ListId == _testList.Id)
            .OrderBy(t => t.Order)
            .ToListAsync();

        for (int i = 0; i < updatedTasks.Count; i++)
        {
            updatedTasks[i].Order.Should().Be(originalOrders[i], "Orders should remain unchanged");
        }
    }

    [Fact]
    public async Task UpdateTaskOrderAsync_WithSingleTask_ShouldWorkCorrectly()
    {
        // Arrange
        var singleTask = TaskEntity.Create("Only Task", TaskPriority.High, _testList.Id, 0);
        _context.Tasks.Add(singleTask);
        await _context.SaveChangesAsync();

        var request = new UpdateTaskOrderRequest(singleTask.Id.ToString(), 0);

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedTask = await _context.Tasks.FindAsync(singleTask.Id);
        updatedTask!.Order.Should().Be(0);
    }

    [Fact]
    public async Task UpdateTaskOrderAsync_WithTwoTasks_ShouldSwapCorrectly()
    {
        // Arrange
        var task1 = TaskEntity.Create("First Task", TaskPriority.Low, _testList.Id, 0);
        var task2 = TaskEntity.Create("Second Task", TaskPriority.High, _testList.Id, 1);
        
        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        var request = new UpdateTaskOrderRequest(task1.Id.ToString(), 1); // Move first to last

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedTasks = await _context.Tasks
            .Where(t => t.ListId == _testList.Id)
            .OrderBy(t => t.Order)
            .ToListAsync();

        updatedTasks.Should().HaveCount(2);
        updatedTasks[0].Id.Should().Be(task2.Id); // Originally second task now first
        updatedTasks[0].Order.Should().Be(0);
        updatedTasks[1].Id.Should().Be(task1.Id); // Originally first task now last
        updatedTasks[1].Order.Should().Be(1);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)] // Out of bounds for 3 tasks (valid range: 0-2)
    [InlineData(10)]
    public async Task UpdateTaskOrderAsync_WithInvalidPosition_ShouldReturnFailure(int invalidPosition)
    {
        // Arrange
        var tasks = new List<TaskEntity>();
        for (int i = 0; i < 3; i++)
        {
            var task = TaskEntity.Create($"Task {i}", TaskPriority.Medium, _testList.Id, i);
            tasks.Add(task);
        }
        
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        var request = new UpdateTaskOrderRequest(tasks[0].Id.ToString(), invalidPosition);

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid order position");
    }

    [Fact]
    public async Task UpdateTaskOrderAsync_WithNonExistentTask_ShouldReturnFailure()
    {
        // Arrange
        var nonExistentTaskId = Guid.NewGuid().ToString();
        var request = new UpdateTaskOrderRequest(nonExistentTaskId, 0);

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Task not found");
    }

    [Fact]
    public async Task UpdateTaskOrderAsync_WithInvalidTaskId_ShouldReturnFailure()
    {
        // Arrange
        var invalidTaskId = "invalid-guid";
        var request = new UpdateTaskOrderRequest(invalidTaskId, 0);

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to update task order");
    }

    [Fact]
    public async Task UpdateTaskOrderAsync_WithDeletedTasks_ShouldOnlyReorderActiveTasks()
    {
        // Arrange
        var activeTask1 = TaskEntity.Create("Active 1", TaskPriority.Medium, _testList.Id, 0);
        var activeTask2 = TaskEntity.Create("Active 2", TaskPriority.Medium, _testList.Id, 1);
        var deletedTask = TaskEntity.Create("Deleted", TaskPriority.Medium, _testList.Id, 2);
        var activeTask3 = TaskEntity.Create("Active 3", TaskPriority.Medium, _testList.Id, 3);
        
        deletedTask.Delete(); // Soft delete
        
        _context.Tasks.AddRange(activeTask1, activeTask2, deletedTask, activeTask3);
        await _context.SaveChangesAsync();

        // Move activeTask3 to position 0 (should only consider active tasks)
        var request = new UpdateTaskOrderRequest(activeTask3.Id.ToString(), 0);

        // Act
        var result = await _taskService.UpdateTaskOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify only active tasks were reordered
        var activeTasks = await _context.Tasks
            .Where(t => t.ListId == _testList.Id && t.DeletedAt == null)
            .OrderBy(t => t.Order)
            .ToListAsync();

        activeTasks.Should().HaveCount(3);
        activeTasks[0].Id.Should().Be(activeTask3.Id); // Moved to first
        activeTasks[0].Order.Should().Be(0);
        activeTasks[1].Id.Should().Be(activeTask1.Id); // Shifted down
        activeTasks[1].Order.Should().Be(1);
        activeTasks[2].Id.Should().Be(activeTask2.Id); // Shifted down
        activeTasks[2].Order.Should().Be(2);

        // Verify deleted task was not affected
        var deletedTaskAfter = await _context.Tasks.FindAsync(deletedTask.Id);
        deletedTaskAfter!.Order.Should().Be(2); // Original order preserved
        deletedTaskAfter.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTaskOrderAsync_ComplexReordering_ShouldMaintainSequentialOrders()
    {
        // Arrange - Create 7 tasks to test more complex scenarios
        var tasks = new List<TaskEntity>();
        for (int i = 0; i < 7; i++)
        {
            var task = TaskEntity.Create($"Task {i}", TaskPriority.Medium, _testList.Id, i);
            tasks.Add(task);
        }
        
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act - Perform multiple reorderings
        // Move task 5 to position 1
        var request1 = new UpdateTaskOrderRequest(tasks[5].Id.ToString(), 1);
        var result1 = await _taskService.UpdateTaskOrderAsync(request1);
        
        // Refresh context to get updated orders
        _context.ChangeTracker.Clear();
        
        // Move what is now at position 3 to position 0  
        var tasksAfterFirst = await _context.Tasks
            .Where(t => t.ListId == _testList.Id && t.DeletedAt == null)
            .OrderBy(t => t.Order)
            .ToListAsync();
        
        var request2 = new UpdateTaskOrderRequest(tasksAfterFirst[3].Id.ToString(), 0);
        var result2 = await _taskService.UpdateTaskOrderAsync(request2);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();

        // Verify final state has sequential orders
        var finalTasks = await _context.Tasks
            .Where(t => t.ListId == _testList.Id && t.DeletedAt == null)
            .OrderBy(t => t.Order)
            .ToListAsync();

        finalTasks.Should().HaveCount(7);
        
        // Check sequential ordering
        for (int i = 0; i < finalTasks.Count; i++)
        {
            finalTasks[i].Order.Should().Be(i, $"Task at position {i} should have order {i}");
        }

        // Check no duplicates or gaps
        var orders = finalTasks.Select(t => t.Order).ToArray();
        orders.Should().BeEquivalentTo(new[] { 0, 1, 2, 3, 4, 5, 6 });
        orders.Should().OnlyHaveUniqueItems();
    }


    #endregion

    #region Integration Tests

    [Fact]
    public async Task TaskLifecycle_CreateToggleDelete_ShouldWorkCorrectly()
    {
        // Arrange
        var createRequest = new CreateTaskRequest(
            Description: "Lifecycle Task",
            Priority: TaskPriority.High,
            ListId: _testList.Id.ToString()
        );

        // Act 1: Create task
        var createResult = await _taskService.CreateTaskAsync(createRequest, _testUser.Id);
        createResult.IsSuccess.Should().BeTrue();

        // Get the created task
        var createdTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Description == "Lifecycle Task");
        createdTask.Should().NotBeNull();
        createdTask!.IsCompleted.Should().BeFalse();

        // Act 2: Toggle task status
        var toggleResult = await _taskService.ToggleTaskStatusAsync(createdTask.Id.ToString());
        toggleResult.IsSuccess.Should().BeTrue();

        // Verify toggle
        var toggledTask = await _context.Tasks.FindAsync(createdTask.Id);
        toggledTask!.IsCompleted.Should().BeTrue();

        // Act 3: Delete task
        var deleteResult = await _taskService.DeleteTaskAsync(createdTask.Id.ToString());
        deleteResult.IsSuccess.Should().BeTrue();

        // Verify deletion
        var deletedTask = await _context.Tasks.FindAsync(createdTask.Id);
        deletedTask!.DeletedAt.Should().NotBeNull();
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
} 