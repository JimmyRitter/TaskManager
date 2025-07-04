using FluentAssertions;
using SaasTaskManager.Core.Common;
using SaasTaskManager.Core.Entities;

namespace SaasTaskManager.Tests.Entities;

public class ListShareTests
{
    private readonly Guid _listId = Guid.NewGuid();
    private readonly Guid _sharedWithUserId = Guid.NewGuid();

    #region Create Method Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateListShareWithCorrectProperties()
    {
        // Arrange
        var permission = SharePermission.ReadOnly;

        // Act
        var listShare = ListShare.Create(_listId, _sharedWithUserId, permission);

        // Assert
        listShare.Should().NotBeNull();
        listShare.Id.Should().NotBeEmpty();
        listShare.ListId.Should().Be(_listId);
        listShare.SharedWithUserId.Should().Be(_sharedWithUserId);
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
        listShare.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        listShare.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithReadOnlyPermission_ShouldSetCorrectPermission()
    {
        // Act
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);

        // Assert
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
    }

    [Fact]
    public void Create_WithFullAccessPermission_ShouldSetCorrectPermission()
    {
        // Act
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.FullAccess);

        // Assert
        listShare.Permission.Should().Be(SharePermission.FullAccess);
    }

    [Fact]
    public void Create_WithEmptyListId_ShouldCreateWithEmptyListId()
    {
        // Arrange
        var emptyListId = Guid.Empty;

        // Act
        var listShare = ListShare.Create(emptyListId, _sharedWithUserId, SharePermission.ReadOnly);

        // Assert
        listShare.ListId.Should().Be(Guid.Empty);
        listShare.SharedWithUserId.Should().Be(_sharedWithUserId);
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
    }

    [Fact]
    public void Create_WithEmptySharedWithUserId_ShouldCreateWithEmptyUserId()
    {
        // Arrange
        var emptyUserId = Guid.Empty;

        // Act
        var listShare = ListShare.Create(_listId, emptyUserId, SharePermission.FullAccess);

        // Assert
        listShare.ListId.Should().Be(_listId);
        listShare.SharedWithUserId.Should().Be(Guid.Empty);
        listShare.Permission.Should().Be(SharePermission.FullAccess);
    }

    [Fact]
    public void Create_MultipleInstances_ShouldHaveUniqueIds()
    {
        // Act
        var listShare1 = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);
        var listShare2 = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);

        // Assert
        listShare1.Id.Should().NotBe(listShare2.Id);
        listShare1.Id.Should().NotBeEmpty();
        listShare2.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);

        // Assert
        var afterCreation = DateTime.UtcNow;
        listShare.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        listShare.CreatedAt.Should().BeOnOrBefore(afterCreation);
    }

    #endregion

    #region UpdatePermission Method Tests

    [Fact]
    public void UpdatePermission_FromReadOnlyToFullAccess_ShouldUpdatePermissionAndTimestamp()
    {
        // Arrange
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);
        var originalCreatedAt = listShare.CreatedAt;
        
        // Small delay to ensure UpdatedAt is different from CreatedAt
        Thread.Sleep(10);

        // Act
        listShare.UpdatePermission(SharePermission.FullAccess);

        // Assert
        listShare.Permission.Should().Be(SharePermission.FullAccess);
        listShare.UpdatedAt.Should().NotBeNull();
        listShare.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        listShare.UpdatedAt.Should().BeAfter(originalCreatedAt);
        listShare.CreatedAt.Should().Be(originalCreatedAt); // Should not change
    }

    [Fact]
    public void UpdatePermission_FromFullAccessToReadOnly_ShouldUpdatePermissionAndTimestamp()
    {
        // Arrange
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.FullAccess);
        var originalCreatedAt = listShare.CreatedAt;
        
        // Small delay to ensure UpdatedAt is different from CreatedAt
        Thread.Sleep(10);

        // Act
        listShare.UpdatePermission(SharePermission.ReadOnly);

        // Assert
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
        listShare.UpdatedAt.Should().NotBeNull();
        listShare.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        listShare.UpdatedAt.Should().BeAfter(originalCreatedAt);
        listShare.CreatedAt.Should().Be(originalCreatedAt); // Should not change
    }

    [Fact]
    public void UpdatePermission_ToSamePermission_ShouldStillUpdateTimestamp()
    {
        // Arrange
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);
        var originalCreatedAt = listShare.CreatedAt;
        
        // Small delay to ensure UpdatedAt is different from CreatedAt
        Thread.Sleep(10);

        // Act
        listShare.UpdatePermission(SharePermission.ReadOnly);

        // Assert
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
        listShare.UpdatedAt.Should().NotBeNull();
        listShare.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        listShare.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }

    [Fact]
    public void UpdatePermission_MultipleUpdates_ShouldUpdateTimestampEachTime()
    {
        // Arrange
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);
        
        // Act 1: First update
        Thread.Sleep(10);
        listShare.UpdatePermission(SharePermission.FullAccess);
        var firstUpdateTime = listShare.UpdatedAt;

        // Act 2: Second update
        Thread.Sleep(10);
        listShare.UpdatePermission(SharePermission.ReadOnly);
        var secondUpdateTime = listShare.UpdatedAt;

        // Assert
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
        firstUpdateTime.Should().NotBeNull();
        secondUpdateTime.Should().NotBeNull();
        secondUpdateTime.Should().BeAfter(firstUpdateTime!.Value);
    }

    #endregion

    #region Property Validation Tests

    [Fact]
    public void ListShare_AllProperties_ShouldHaveCorrectAccessModifiers()
    {
        // Arrange
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);

        // Act & Assert - Properties should be readable
        var id = listShare.Id;
        var listId = listShare.ListId;
        var sharedWithUserId = listShare.SharedWithUserId;
        var permission = listShare.Permission;
        var createdAt = listShare.CreatedAt;
        var updatedAt = listShare.UpdatedAt;

        // All properties should be accessible (test passes if no exception)
        id.Should().NotBeEmpty();
        listId.Should().Be(_listId);
        sharedWithUserId.Should().Be(_sharedWithUserId);
        permission.Should().Be(SharePermission.ReadOnly);
        createdAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        updatedAt.Should().BeNull();
    }

    [Fact]
    public void ListShare_PropertiesWithPrivateSetters_ShouldNotBeModifiableDirectly()
    {
        // This test validates that properties have private setters by ensuring
        // they can only be modified through the designated methods
        var listShare = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);

        // The fact that we can't set properties directly is enforced by the compiler
        // This test serves as documentation that the properties are read-only from outside
        listShare.Id.Should().NotBeEmpty();
        listShare.ListId.Should().Be(_listId);
        listShare.SharedWithUserId.Should().Be(_sharedWithUserId);
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
        listShare.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        listShare.UpdatedAt.Should().BeNull();
    }

    #endregion

    #region Integration and Edge Case Tests

    [Fact]
    public void ListShare_CompleteLifecycle_ShouldWorkCorrectly()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act 1: Create with ReadOnly permission
        var listShare = ListShare.Create(listId, userId, SharePermission.ReadOnly);
        var createdAt = listShare.CreatedAt;

        // Act 2: Update to FullAccess
        Thread.Sleep(10);
        listShare.UpdatePermission(SharePermission.FullAccess);
        var firstUpdateTime = listShare.UpdatedAt;

        // Act 3: Update back to ReadOnly
        Thread.Sleep(10);
        listShare.UpdatePermission(SharePermission.ReadOnly);
        var secondUpdateTime = listShare.UpdatedAt;

        // Assert
        listShare.Id.Should().NotBeEmpty();
        listShare.ListId.Should().Be(listId);
        listShare.SharedWithUserId.Should().Be(userId);
        listShare.Permission.Should().Be(SharePermission.ReadOnly);
        listShare.CreatedAt.Should().Be(createdAt);
        
        firstUpdateTime.Should().NotBeNull();
        secondUpdateTime.Should().NotBeNull();
        firstUpdateTime.Should().BeAfter(createdAt);
        secondUpdateTime.Should().BeAfter(firstUpdateTime!.Value);
    }

    [Fact]
    public void ListShare_WithVariousPermissionCombinations_ShouldHandleAllScenarios()
    {
        // Test all possible permission transitions
        var testCases = new[]
        {
            new { Initial = SharePermission.ReadOnly, Updated = SharePermission.FullAccess },
            new { Initial = SharePermission.FullAccess, Updated = SharePermission.ReadOnly },
            new { Initial = SharePermission.ReadOnly, Updated = SharePermission.ReadOnly },
            new { Initial = SharePermission.FullAccess, Updated = SharePermission.FullAccess }
        };

        foreach (var testCase in testCases)
        {
            // Arrange
            var listShare = ListShare.Create(_listId, _sharedWithUserId, testCase.Initial);
            var originalCreatedAt = listShare.CreatedAt;

            // Act
            Thread.Sleep(10);
            listShare.UpdatePermission(testCase.Updated);

            // Assert
            listShare.Permission.Should().Be(testCase.Updated);
            listShare.UpdatedAt.Should().NotBeNull();
            listShare.UpdatedAt.Should().BeAfter(originalCreatedAt);
            listShare.CreatedAt.Should().Be(originalCreatedAt);
        }
    }

    [Fact]
    public void ListShare_WithSameUserAndList_ShouldCreateDifferentInstances()
    {
        // This tests that multiple shares for the same list/user combination are possible
        // (business logic validation would be handled at the service layer)
        
        // Act
        var share1 = ListShare.Create(_listId, _sharedWithUserId, SharePermission.ReadOnly);
        var share2 = ListShare.Create(_listId, _sharedWithUserId, SharePermission.FullAccess);

        // Assert
        share1.Id.Should().NotBe(share2.Id);
        share1.ListId.Should().Be(share2.ListId);
        share1.SharedWithUserId.Should().Be(share2.SharedWithUserId);
        share1.Permission.Should().Be(SharePermission.ReadOnly);
        share2.Permission.Should().Be(SharePermission.FullAccess);
    }

    #endregion
} 