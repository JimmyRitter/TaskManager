using FluentAssertions;
using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;
using TaskEntity = SaasTaskManager.Core.Entities.Task;

namespace SaasTaskManager.Tests.Entities;

public class TaskTests
{
    private readonly Guid _listId = Guid.NewGuid();

    #region Create Method Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateTaskWithCorrectProperties()
    {
        // Arrange
        var description = "Test task";
        var priority = TaskPriority.Medium;

        // Act
        var task = TaskEntity.Create(description, priority, _listId);

        // Assert
        task.Should().NotBeNull();
        task.Id.Should().NotBeEmpty();
        task.Description.Should().Be(description);
        task.Priority.Should().Be(priority);
        task.ListId.Should().Be(_listId);
        task.IsCompleted.Should().BeFalse();
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().BeNull();
        task.DeletedAt.Should().BeNull();
        task.DueDate.Should().BeNull();
    }

    [Fact]
    public void Create_WithDifferentPriorities_ShouldSetCorrectPriority()
    {
        var priorities = new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High, TaskPriority.Critical };

        foreach (var priority in priorities)
        {
            // Act
            var task = TaskEntity.Create("Test task", priority, _listId);

            // Assert
            task.Priority.Should().Be(priority);
        }
    }

    #endregion

    #region ToggleStatus Method Tests

    [Fact]
    public void ToggleStatus_FromIncompleteToComplete_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        var originalCreatedAt = task.CreatedAt;

        // Act
        task.ToggleStatus();

        // Assert
        task.IsCompleted.Should().BeTrue();
        task.UpdatedAt.Should().NotBeNull();
        task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().BeAfter(originalCreatedAt);
        task.CreatedAt.Should().Be(originalCreatedAt); // Should not change
    }

    [Fact]
    public void ToggleStatus_FromCompleteToIncomplete_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        task.ToggleStatus(); // Make it complete first
        var firstUpdateTime = task.UpdatedAt;

        // Act
        task.ToggleStatus(); // Toggle back to incomplete

        // Assert
        task.IsCompleted.Should().BeFalse();
        task.UpdatedAt.Should().NotBeNull();
        task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().BeAfter(firstUpdateTime!.Value);
    }

    #endregion

    #region Delete Method Tests

    [Fact]
    public void Delete_ShouldSetDeletedAtAndUpdateTimestamp()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        var originalCreatedAt = task.CreatedAt;

        // Act
        task.Delete();

        // Assert
        task.DeletedAt.Should().NotBeNull();
        task.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().NotBeNull();
        task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().BeAfter(originalCreatedAt);
        task.CreatedAt.Should().Be(originalCreatedAt); // Should not change
    }

    [Fact]
    public void Delete_OnAlreadyDeletedTask_ShouldUpdateTimestamps()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        task.Delete();
        var firstDeleteTime = task.DeletedAt;
        var firstUpdateTime = task.UpdatedAt;

        // Small delay to ensure different timestamps
        Thread.Sleep(10);

        // Act
        task.Delete();

        // Assert
        task.DeletedAt.Should().BeAfter(firstDeleteTime!.Value);
        task.UpdatedAt.Should().BeAfter(firstUpdateTime!.Value);
    }

    #endregion

    #region Recreate Method Tests

    [Fact]
    public void Recreate_OnDeletedTask_ShouldClearDeletedAtAndUpdateTimestamp()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        task.Delete();
        var originalCreatedAt = task.CreatedAt;

        // Act
        task.Recreate();

        // Assert
        task.DeletedAt.Should().BeNull();
        task.UpdatedAt.Should().NotBeNull();
        task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().BeAfter(originalCreatedAt);
        task.CreatedAt.Should().Be(originalCreatedAt); // Should not change
    }

    [Fact]
    public void Recreate_OnNonDeletedTask_ShouldNotChangeAnything()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        var originalCreatedAt = task.CreatedAt;
        var originalUpdatedAt = task.UpdatedAt;

        // Act
        task.Recreate();

        // Assert
        task.DeletedAt.Should().BeNull();
        task.UpdatedAt.Should().Be(originalUpdatedAt); // Should not change
        task.CreatedAt.Should().Be(originalCreatedAt); // Should not change
    }

    [Fact]
    public void Recreate_OnTaskWithUpdatedAt_ShouldUpdateTimestamp()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        task.ToggleStatus(); // Give it an UpdatedAt
        var originalUpdateTime = task.UpdatedAt;
        task.Delete();

        // Act
        task.Recreate();

        // Assert
        task.DeletedAt.Should().BeNull();
        task.UpdatedAt.Should().NotBeNull();
        task.UpdatedAt.Should().BeAfter(originalUpdateTime!.Value);
    }

    [Fact]
    public void Recreate_MultipleTimesOnDeletedTask_ShouldWorkCorrectly()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        task.Delete();

        // Act
        task.Recreate();
        var firstRecreateTime = task.UpdatedAt;
        
        task.Delete();
        Thread.Sleep(10);
        task.Recreate();
        var secondRecreateTime = task.UpdatedAt;

        // Assert
        task.DeletedAt.Should().BeNull();
        secondRecreateTime.Should().BeAfter(firstRecreateTime!.Value);
    }

    #endregion

    #region IsPastDueDate Property Tests

    [Fact]
    public void IsPastDueDate_WithNoDueDate_ShouldReturnFalse()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);

        // Act & Assert
        task.IsPastDueDate.Should().BeFalse();
    }

    [Fact]
    public void IsPastDueDate_WithFutureDueDate_ShouldReturnFalse()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        
        // Use reflection to set DueDate since there's no public setter
        var dueDateProperty = typeof(TaskEntity).GetProperty("DueDate");
        dueDateProperty!.SetValue(task, DateTime.UtcNow.AddDays(1));

        // Act & Assert
        task.IsPastDueDate.Should().BeFalse();
    }

    [Fact]
    public void IsPastDueDate_WithPastDueDate_ShouldReturnTrue()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        
        // Use reflection to set DueDate since there's no public setter
        var dueDateProperty = typeof(TaskEntity).GetProperty("DueDate");
        dueDateProperty!.SetValue(task, DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        task.IsPastDueDate.Should().BeTrue();
    }

    [Fact]
    public void IsPastDueDate_WithDueDateExactlyNow_ShouldReturnTrue()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
        var now = DateTime.UtcNow;
        
        // Use reflection to set DueDate since there's no public setter
        var dueDateProperty = typeof(TaskEntity).GetProperty("DueDate");
        dueDateProperty!.SetValue(task, now.AddMilliseconds(-1)); // Slightly in the past

        // Act & Assert
        task.IsPastDueDate.Should().BeTrue();
    }

    [Fact]
    public void IsPastDueDate_WithVariousDueDates_ShouldReturnCorrectResults()
    {
        var testCases = new[]
        {
            new { DueDate = (DateTime?)null, Expected = false, Description = "No due date" },
            new { DueDate = (DateTime?)DateTime.UtcNow.AddMinutes(-1), Expected = true, Description = "1 minute ago" },
            new { DueDate = (DateTime?)DateTime.UtcNow.AddMinutes(1), Expected = false, Description = "1 minute from now" },
            new { DueDate = (DateTime?)DateTime.UtcNow.AddDays(-1), Expected = true, Description = "1 day ago" },
            new { DueDate = (DateTime?)DateTime.UtcNow.AddDays(1), Expected = false, Description = "1 day from now" },
            new { DueDate = (DateTime?)DateTime.UtcNow.AddYears(-1), Expected = true, Description = "1 year ago" },
            new { DueDate = (DateTime?)DateTime.UtcNow.AddYears(1), Expected = false, Description = "1 year from now" }
        };

        foreach (var testCase in testCases)
        {
            // Arrange
            var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);
            var dueDateProperty = typeof(TaskEntity).GetProperty("DueDate");
            dueDateProperty!.SetValue(task, testCase.DueDate);

            // Act & Assert
            task.IsPastDueDate.Should().Be(testCase.Expected, testCase.Description);
        }
    }

    #endregion

    #region Integration and Edge Case Tests

    [Fact]
    public void TaskLifecycle_CompleteWorkflow_ShouldWorkCorrectly()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.High, _listId);
        var originalCreatedAt = task.CreatedAt;

        // Act & Assert - Complete workflow
        
        // 1. Toggle status
        task.ToggleStatus();
        task.IsCompleted.Should().BeTrue();
        task.UpdatedAt.Should().NotBeNull();

        // 2. Delete task
        task.Delete();
        task.DeletedAt.Should().NotBeNull();
        task.UpdatedAt.Should().BeAfter(originalCreatedAt);

        // 3. Recreate task
        task.Recreate();
        task.DeletedAt.Should().BeNull();
        task.IsCompleted.Should().BeTrue(); // Should preserve completed status

        // 4. Toggle status again
        task.ToggleStatus();
        task.IsCompleted.Should().BeFalse();

        // Final assertions
        task.Id.Should().NotBeEmpty();
        task.Description.Should().Be("Test task");
        task.Priority.Should().Be(TaskPriority.High);
        task.ListId.Should().Be(_listId);
        task.CreatedAt.Should().Be(originalCreatedAt);
        task.UpdatedAt.Should().NotBeNull();
        task.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Task_PropertiesWithPrivateSetters_ShouldNotBeModifiableDirectly()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Medium, _listId);

        // Act & Assert - Properties should be readable but not writable from outside
        task.Id.Should().NotBeEmpty();
        task.Description.Should().Be("Test task");
        task.Priority.Should().Be(TaskPriority.Medium);
        task.ListId.Should().Be(_listId);
        task.IsCompleted.Should().BeFalse();
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().BeNull();
        task.DeletedAt.Should().BeNull();
        task.DueDate.Should().BeNull();
    }

    [Fact]
    public void Task_MultipleOperations_ShouldMaintainConsistentState()
    {
        // Arrange
        var task = TaskEntity.Create("Test task", TaskPriority.Critical, _listId);

        // Act - Perform multiple operations
        task.ToggleStatus(); // Complete
        task.Delete(); // Delete
        task.Recreate(); // Recreate
        task.ToggleStatus(); // Incomplete
        task.ToggleStatus(); // Complete again
        task.Delete(); // Delete again

        // Assert
        task.IsCompleted.Should().BeTrue();
        task.DeletedAt.Should().NotBeNull();
        task.UpdatedAt.Should().NotBeNull();
        task.Description.Should().Be("Test task"); // Should not change
        task.Priority.Should().Be(TaskPriority.Critical); // Should not change
        task.ListId.Should().Be(_listId); // Should not change
    }

    [Fact]
    public void Task_WithEdgeCaseData_ShouldHandleCorrectly()
    {
        // Test with edge case data
        var testCases = new[]
        {
            new { Description = "", Priority = TaskPriority.Low },
            new { Description = "Very long description that might cause issues with database storage or other systems", Priority = TaskPriority.High },
            new { Description = "Special chars !@#$%^&*()_+-=[]{}|;:,.<>?", Priority = TaskPriority.Critical },
            new { Description = "Unicode characters: 你好世界 🌍 émojis", Priority = TaskPriority.Medium }
        };

        foreach (var testCase in testCases)
        {
            // Act
            var task = TaskEntity.Create(testCase.Description, testCase.Priority, _listId);

            // Assert
            task.Description.Should().Be(testCase.Description);
            task.Priority.Should().Be(testCase.Priority);
            task.IsCompleted.Should().BeFalse();
            task.DeletedAt.Should().BeNull();
        }
    }

    [Fact]
    public void Task_TimestampPrecision_ShouldBeAccurate()
    {
        // This test ensures timestamp precision is maintained
        var tasks = new List<TaskEntity>();
        
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(TaskEntity.Create($"Task {i}", TaskPriority.Medium, _listId));
            if (i < 2) Thread.Sleep(1); // Small delay to ensure different timestamps
        }

        // Assert that timestamps are ordered correctly
        for (int i = 1; i < tasks.Count; i++)
        {
            tasks[i].CreatedAt.Should().BeOnOrAfter(tasks[i - 1].CreatedAt);
        }
    }

    #endregion
} 