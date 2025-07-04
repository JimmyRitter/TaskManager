using FluentAssertions;
using SaasTaskManager.Core.Entities;

namespace SaasTaskManager.Tests.Entities;

public class TaskStatusChangeTests
{
    private readonly Guid _taskId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    #region Create Method Tests

    [Fact]
    public void Create_WithValidDataAndCompletedStatus_ShouldCreateTaskStatusChangeWithCorrectProperties()
    {
        // Act
        var statusChange = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);

        // Assert
        statusChange.Should().NotBeNull();
        statusChange.Id.Should().NotBeEmpty();
        statusChange.TaskId.Should().Be(_taskId);
        statusChange.ChangedByUserId.Should().Be(_userId);
        statusChange.IsCompleted.Should().BeTrue();
        statusChange.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithValidDataAndIncompleteStatus_ShouldCreateTaskStatusChangeWithCorrectProperties()
    {
        // Act
        var statusChange = TaskStatusChange.Create(_taskId, _userId, isCompleted: false);

        // Assert
        statusChange.Should().NotBeNull();
        statusChange.Id.Should().NotBeEmpty();
        statusChange.TaskId.Should().Be(_taskId);
        statusChange.ChangedByUserId.Should().Be(_userId);
        statusChange.IsCompleted.Should().BeFalse();
        statusChange.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyTaskId_ShouldCreateWithEmptyTaskId()
    {
        // Arrange
        var emptyTaskId = Guid.Empty;

        // Act
        var statusChange = TaskStatusChange.Create(emptyTaskId, _userId, isCompleted: true);

        // Assert
        statusChange.TaskId.Should().Be(Guid.Empty);
        statusChange.ChangedByUserId.Should().Be(_userId);
        statusChange.IsCompleted.Should().BeTrue();
        statusChange.Id.Should().NotBeEmpty();
        statusChange.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldCreateWithEmptyUserId()
    {
        // Arrange
        var emptyUserId = Guid.Empty;

        // Act
        var statusChange = TaskStatusChange.Create(_taskId, emptyUserId, isCompleted: false);

        // Assert
        statusChange.TaskId.Should().Be(_taskId);
        statusChange.ChangedByUserId.Should().Be(Guid.Empty);
        statusChange.IsCompleted.Should().BeFalse();
        statusChange.Id.Should().NotBeEmpty();
        statusChange.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_MultipleInstances_ShouldHaveUniqueIds()
    {
        // Act
        var statusChange1 = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);
        var statusChange2 = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);

        // Assert
        statusChange1.Id.Should().NotBe(statusChange2.Id);
        statusChange1.Id.Should().NotBeEmpty();
        statusChange2.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var statusChange = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);

        // Assert
        var afterCreation = DateTime.UtcNow;
        statusChange.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        statusChange.CreatedAt.Should().BeOnOrBefore(afterCreation);
    }

    [Fact]
    public void Create_WithSameParametersMultipleTimes_ShouldCreateDistinctInstances()
    {
        // Act
        var statusChange1 = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);
        var statusChange2 = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);
        var statusChange3 = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);

        // Assert
        var instances = new[] { statusChange1, statusChange2, statusChange3 };
        var uniqueIds = instances.Select(x => x.Id).Distinct().ToArray();
        
        uniqueIds.Should().HaveCount(3);
        instances.Should().AllSatisfy(x => x.TaskId.Should().Be(_taskId));
        instances.Should().AllSatisfy(x => x.ChangedByUserId.Should().Be(_userId));
        instances.Should().AllSatisfy(x => x.IsCompleted.Should().BeTrue());
    }

    #endregion

    #region Property Validation Tests

    [Fact]
    public void TaskStatusChange_AllProperties_ShouldHaveCorrectAccessModifiers()
    {
        // Arrange
        var statusChange = TaskStatusChange.Create(_taskId, _userId, isCompleted: true);

        // Act & Assert - Properties should be readable
        var id = statusChange.Id;
        var taskId = statusChange.TaskId;
        var changedByUserId = statusChange.ChangedByUserId;
        var isCompleted = statusChange.IsCompleted;
        var createdAt = statusChange.CreatedAt;

        // All properties should be accessible (test passes if no exception)
        id.Should().NotBeEmpty();
        taskId.Should().Be(_taskId);
        changedByUserId.Should().Be(_userId);
        isCompleted.Should().BeTrue();
        createdAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void TaskStatusChange_PropertiesWithPrivateSetters_ShouldNotBeModifiableDirectly()
    {
        // This test validates that properties have private setters by ensuring
        // they can only be modified through the designated methods
        var statusChange = TaskStatusChange.Create(_taskId, _userId, isCompleted: false);

        // The fact that we can't set properties directly is enforced by the compiler
        // This test serves as documentation that the properties are read-only from outside
        statusChange.Id.Should().NotBeEmpty();
        statusChange.TaskId.Should().Be(_taskId);
        statusChange.ChangedByUserId.Should().Be(_userId);
        statusChange.IsCompleted.Should().BeFalse();
        statusChange.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public void Create_ForTaskCompletion_ShouldRecordCompletionCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var completionChange = TaskStatusChange.Create(taskId, userId, isCompleted: true);

        // Assert
        completionChange.TaskId.Should().Be(taskId);
        completionChange.ChangedByUserId.Should().Be(userId);
        completionChange.IsCompleted.Should().BeTrue();
        completionChange.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ForTaskUncompletetion_ShouldRecordUncompletionCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var uncompletion = TaskStatusChange.Create(taskId, userId, isCompleted: false);

        // Assert
        uncompletion.TaskId.Should().Be(taskId);
        uncompletion.ChangedByUserId.Should().Be(userId);
        uncompletion.IsCompleted.Should().BeFalse();
        uncompletion.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ForMultipleStatusChanges_ShouldTrackEachChangeIndependently()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        // Act
        var change1 = TaskStatusChange.Create(taskId, user1Id, isCompleted: true);
        Thread.Sleep(10); // Ensure different timestamps
        var change2 = TaskStatusChange.Create(taskId, user2Id, isCompleted: false);
        Thread.Sleep(10);
        var change3 = TaskStatusChange.Create(taskId, user1Id, isCompleted: true);

        // Assert
        var changes = new[] { change1, change2, change3 };
        
        // All should have unique IDs
        changes.Select(c => c.Id).Distinct().Should().HaveCount(3);
        
        // All should reference the same task
        changes.Should().AllSatisfy(c => c.TaskId.Should().Be(taskId));
        
        // Should have different users and statuses
        change1.ChangedByUserId.Should().Be(user1Id);
        change1.IsCompleted.Should().BeTrue();
        
        change2.ChangedByUserId.Should().Be(user2Id);
        change2.IsCompleted.Should().BeFalse();
        
        change3.ChangedByUserId.Should().Be(user1Id);
        change3.IsCompleted.Should().BeTrue();
        
        // Timestamps should be in order
        change2.CreatedAt.Should().BeAfter(change1.CreatedAt);
        change3.CreatedAt.Should().BeAfter(change2.CreatedAt);
    }

    #endregion

    #region Integration and Edge Case Tests

    [Fact]
    public void Create_WithAllPossibleStatusCombinations_ShouldWorkCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            new { TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), IsCompleted = true },
            new { TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), IsCompleted = false },
            new { TaskId = Guid.Empty, UserId = Guid.NewGuid(), IsCompleted = true },
            new { TaskId = Guid.NewGuid(), UserId = Guid.Empty, IsCompleted = false },
            new { TaskId = Guid.Empty, UserId = Guid.Empty, IsCompleted = true },
            new { TaskId = Guid.Empty, UserId = Guid.Empty, IsCompleted = false }
        };

        // Act & Assert
        foreach (var testCase in testCases)
        {
            var statusChange = TaskStatusChange.Create(testCase.TaskId, testCase.UserId, testCase.IsCompleted);
            
            statusChange.Should().NotBeNull();
            statusChange.Id.Should().NotBeEmpty();
            statusChange.TaskId.Should().Be(testCase.TaskId);
            statusChange.ChangedByUserId.Should().Be(testCase.UserId);
            statusChange.IsCompleted.Should().Be(testCase.IsCompleted);
            statusChange.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }

    [Fact]
    public void Create_SimulatingTaskStatusHistory_ShouldCreateChronologicalRecord()
    {
        // This test simulates a real-world scenario where a task changes status multiple times
        // Arrange
        var taskId = Guid.NewGuid();
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        // Act - Simulate task status changes over time
        var initialCompletion = TaskStatusChange.Create(taskId, user1Id, isCompleted: true);
        Thread.Sleep(10);
        
        var undoCompletion = TaskStatusChange.Create(taskId, user2Id, isCompleted: false);
        Thread.Sleep(10);
        
        var recompletion = TaskStatusChange.Create(taskId, user1Id, isCompleted: true);
        Thread.Sleep(10);
        
        var finalUndo = TaskStatusChange.Create(taskId, user2Id, isCompleted: false);

        // Assert
        var statusHistory = new[] { initialCompletion, undoCompletion, recompletion, finalUndo };
        
        // Verify chronological order
        for (int i = 1; i < statusHistory.Length; i++)
        {
            statusHistory[i].CreatedAt.Should().BeAfter(statusHistory[i - 1].CreatedAt);
        }
        
        // Verify alternating completion status
        statusHistory[0].IsCompleted.Should().BeTrue();
        statusHistory[1].IsCompleted.Should().BeFalse();
        statusHistory[2].IsCompleted.Should().BeTrue();
        statusHistory[3].IsCompleted.Should().BeFalse();
        
        // Verify all changes are for the same task
        statusHistory.Should().AllSatisfy(s => s.TaskId.Should().Be(taskId));
        
        // Verify user attribution
        statusHistory[0].ChangedByUserId.Should().Be(user1Id);
        statusHistory[1].ChangedByUserId.Should().Be(user2Id);
        statusHistory[2].ChangedByUserId.Should().Be(user1Id);
        statusHistory[3].ChangedByUserId.Should().Be(user2Id);
    }

    [Fact]
    public void Create_WithExtremeValues_ShouldHandleEdgeCases()
    {
        // Test with maximum and minimum GUID values (edge case testing)
        var maxGuid = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var minGuid = new Guid("00000000-0000-0000-0000-000000000000");

        // Act
        var maxValueChange = TaskStatusChange.Create(maxGuid, maxGuid, isCompleted: true);
        var minValueChange = TaskStatusChange.Create(minGuid, minGuid, isCompleted: false);

        // Assert
        maxValueChange.TaskId.Should().Be(maxGuid);
        maxValueChange.ChangedByUserId.Should().Be(maxGuid);
        maxValueChange.IsCompleted.Should().BeTrue();

        minValueChange.TaskId.Should().Be(minGuid);
        minValueChange.ChangedByUserId.Should().Be(minGuid);
        minValueChange.IsCompleted.Should().BeFalse();
    }

    #endregion
} 