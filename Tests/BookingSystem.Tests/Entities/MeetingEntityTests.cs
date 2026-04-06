using BookingSystem.Application.DTOs;
using BookingSystem.Application.Exceptions;
using BookingSystem.Application.Services;
using BookingSystem.Core.Entities;
using BookingSystem.Core.Entities.Aggregates;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BookingSystem.Tests.Entities;

public class MeetingEntityTests
{
    [Fact]
    public void CreateInvitation_NoExistingInvitation_CreatesSuccessfully()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var inviteeId = Guid.NewGuid();
        var inviterId = creatorId;

        // Act
        var invitation = meeting.CreateInvitation(inviteeId, inviterId);

        // Assert
        Assert.NotNull(invitation);
        Assert.Equal(inviteeId, invitation.InviteeId);
        Assert.Equal(inviterId, invitation.InviterId);
        Assert.Equal(InvitationStatus.Pending, invitation.Status);
        Assert.Single(meeting.Invitations);
    }

    [Fact]
    public void CreateInvitation_ExistingPendingInvitation_ThrowsInvalidOperationException()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var inviteeId = Guid.NewGuid();
        var inviterId = creatorId;

        // Создаем первое приглашение
        meeting.CreateInvitation(inviteeId, inviterId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            meeting.CreateInvitation(inviteeId, inviterId));
    }

    [Fact]
    public void CreateInvitation_AfterDeclined_CanCreateNew()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var inviteeId = Guid.NewGuid();
        var inviterId = creatorId;

        // Создаем и отклоняем приглашение
        var invitation = meeting.CreateInvitation(inviteeId, inviterId);
        invitation.Decline();

        // Act - должно успешно создать новое
        var newInvitation = meeting.CreateInvitation(inviteeId, inviterId);

        // Assert
        Assert.NotNull(newInvitation);
        Assert.Equal(2, meeting.Invitations.Count);
    }

    [Fact]
    public void Subscribe_NewUser_AddsToSubscribers()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new AppUser { Id = Guid.NewGuid(), Email = "user@test.com" };

        // Act
        meeting.Subscribe(user);

        // Assert
        Assert.Contains(user, meeting.Subscribers);
        Assert.Single(meeting.Subscribers);
    }

    [Fact]
    public void Subscribe_ExistingUser_DoesNotAddDuplicate()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new AppUser { Id = Guid.NewGuid(), Email = "user@test.com" };

        meeting.Subscribe(user);

        // Act
        meeting.Subscribe(user);

        // Assert
        Assert.Single(meeting.Subscribers);
    }

    [Fact]
    public void Unsubscribe_ExistingUser_RemovesFromSubscribers()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new AppUser { Id = Guid.NewGuid(), Email = "user@test.com" };

        meeting.Subscribe(user);

        // Act
        meeting.Unsubscribe(user);

        // Assert
        Assert.DoesNotContain(user, meeting.Subscribers);
        Assert.Empty(meeting.Subscribers);
    }

    [Fact]
    public void Unsubscribe_NonExistingUser_DoesNothing()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new AppUser { Id = Guid.NewGuid(), Email = "user@test.com" };
        var otherUser = new AppUser { Id = Guid.NewGuid(), Email = "other@test.com" };

        meeting.Subscribe(user);

        // Act
        meeting.Unsubscribe(otherUser);

        // Assert
        Assert.Contains(user, meeting.Subscribers);
        Assert.Single(meeting.Subscribers);
    }

    [Fact]
    public void Cancel_SetsIsCancelledToTrue()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        // Act
        meeting.Cancel();

        // Assert
        Assert.True(meeting.IsCancelled);
    }

    [Fact]
    public void Uncancel_SetsIsCancelledToFalse()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var meeting = new Meeting(creatorId, roomId, "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        meeting.Cancel();

        // Act
        meeting.Uncancel();

        // Assert
        Assert.False(meeting.IsCancelled);
    }

    [Fact]
    public void UpdateTimeRange_UpdatesTimeRange()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var endTime = DateTime.UtcNow.AddHours(1);
        var meeting = new Meeting(creatorId, roomId, "Test", startTime, endTime);

        var newStartTime = DateTime.UtcNow.AddDays(1);
        var newEndTime = DateTime.UtcNow.AddDays(1).AddHours(2);

        // Act
        meeting.UpdateTimeRange(newStartTime, newEndTime);

        // Assert
        Assert.Equal(newStartTime, meeting.TimeRange.Start);
        Assert.Equal(newEndTime, meeting.TimeRange.End);
    }
}

public class MeetingInvitationEntityTests
{
    [Fact]
    public void Accept_PendingInvitation_SetsStatusToAccepted()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);

        // Act
        invitation.Accept();

        // Assert
        Assert.Equal(InvitationStatus.Accepted, invitation.Status);
        Assert.NotNull(invitation.RespondedAt);
    }

    [Fact]
    public void Decline_PendingInvitation_SetsStatusToDeclined()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);

        // Act
        invitation.Decline();

        // Assert
        Assert.Equal(InvitationStatus.Declined, invitation.Status);
        Assert.NotNull(invitation.RespondedAt);
    }

    [Fact]
    public void Cancel_PendingInvitation_SetsStatusToDeclined()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);

        // Act
        invitation.Cancel();

        // Assert
        Assert.Equal(InvitationStatus.Declined, invitation.Status);
        Assert.NotNull(invitation.RespondedAt);
    }

    [Fact]
    public void Accept_AlreadyAccepted_ThrowsInvalidOperationException()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);
        invitation.Accept();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => invitation.Accept());
    }

    [Fact]
    public void Decline_AlreadyDeclined_ThrowsInvalidOperationException()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);
        invitation.Decline();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => invitation.Decline());
    }

    [Fact]
    public void Accept_AfterDeclined_ThrowsInvalidOperationException()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);
        invitation.Decline();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => invitation.Accept());
    }

    [Fact]
    public void Decline_AfterAccepted_ThrowsInvalidOperationException()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);
        invitation.Accept();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => invitation.Decline());
    }

    [Fact]
    public void Constructor_SetsCreatedAtToUtcNow()
    {
        // Arrange
        var beforeCreate = DateTime.UtcNow;
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();

        // Act
        var invitation = new MeetingInvitation(meetingId, inviteeId, inviterId);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.InRange(invitation.CreatedAt, beforeCreate, afterCreate);
    }
}
