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

namespace BookingSystem.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IRoomRepository> _roomRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IInvitationRepository> _invitationRepoMock;
    private readonly Mock<INotificationService> _notificationService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<BookingService>> _loggerMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingRepoMock = new Mock<IBookingRepository>();
        _roomRepoMock = new Mock<IRoomRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _invitationRepoMock = new Mock<IInvitationRepository>();
        _notificationService = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<BookingService>>();

        _bookingService = new BookingService(
            _bookingRepoMock.Object,
            _roomRepoMock.Object,
            _userRepoMock.Object,
            _invitationRepoMock.Object,
            _notificationService.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);
    }

    #region CreateInvitationsAsync Tests

    [Fact]
    public async Task CreateInvitationsAsync_MeetingNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteesIds = new List<Guid> { Guid.NewGuid() };
        var inviterId = Guid.NewGuid();

        _bookingRepoMock.Setup(r => r.GetByIdAsyncWithInclude(meetingId))
            .ReturnsAsync((Meeting?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _bookingService.CreateInvitationsAsync(meetingId, inviteesIds, inviterId));
    }

    [Fact]
    public async Task CreateInvitationsAsync_InviterNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteesIds = new List<Guid> { Guid.NewGuid() };
        var inviterId = Guid.NewGuid();

        var meeting = new Meeting(Guid.NewGuid(), Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        _bookingRepoMock.Setup(r => r.GetByIdAsyncWithInclude(meetingId))
            .ReturnsAsync(meeting);

        _userRepoMock.Setup(r => r.GetByIdAsync(inviterId))
            .ReturnsAsync((AppUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _bookingService.CreateInvitationsAsync(meetingId, inviteesIds, inviterId));
    }

    [Fact]
    public async Task CreateInvitationsAsync_UserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var inviterId = Guid.NewGuid();

        var meeting = new Meeting(Guid.NewGuid(), Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        _bookingRepoMock.Setup(r => r.GetByIdAsyncWithInclude(meetingId))
            .ReturnsAsync(meeting);

        var inviter = new AppUser { Id = inviterId, Email = "inviter@test.com" };
        _userRepoMock.Setup(r => r.GetByIdAsync(inviterId))
            .ReturnsAsync(inviter);

        // Возвращаем только одного пользователя вместо двух
        var foundUsers = new List<AppUser> { new AppUser { Id = inviteesIds[0], Email = "user1@test.com" } };
        _userRepoMock.Setup(r => r.GetByIdsAsync(inviteesIds))
            .ReturnsAsync(foundUsers);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _bookingService.CreateInvitationsAsync(meetingId, inviteesIds, inviterId));
        
        Assert.Contains("One or more users not found", exception.Message);
    }

    [Fact]
    public async Task CreateInvitationsAsync_Success_CreatesInvitations()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviteesIds = new List<Guid> { inviteeId };
        var inviterId = Guid.NewGuid();

        var meeting = new Meeting(inviterId, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        _bookingRepoMock.Setup(r => r.GetByIdAsyncWithInclude(meetingId))
            .ReturnsAsync(meeting);

        var inviter = new AppUser { Id = inviterId, Email = "inviter@test.com" };
        _userRepoMock.Setup(r => r.GetByIdAsync(inviterId))
            .ReturnsAsync(inviter);

        var invitee = new AppUser { Id = inviteeId, Email = "invitee@test.com" };
        _userRepoMock.Setup(r => r.GetByIdsAsync(inviteesIds))
            .ReturnsAsync(new List<AppUser> { invitee });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _bookingService.CreateInvitationsAsync(meetingId, inviteesIds, inviterId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(inviteeId, result[0].InviteeId);
        Assert.Equal(inviterId, result[0].InviterId);
        Assert.Equal("Pending", result[0].Status);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvitationsAsync_DuplicateInvitation_SkipsExisting()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();
        var inviteesIds = new List<Guid> { inviteeId };
        var inviterId = Guid.NewGuid();

        var meeting = new Meeting(inviterId, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        // Создаем существующее приглашение
        var existingInvitation = meeting.CreateInvitation(inviteeId, inviterId);
        
        _bookingRepoMock.Setup(r => r.GetByIdAsyncWithInclude(meetingId))
            .ReturnsAsync(meeting);

        var inviter = new AppUser { Id = inviterId, Email = "inviter@test.com" };
        _userRepoMock.Setup(r => r.GetByIdAsync(inviterId))
            .ReturnsAsync(inviter);

        var invitee = new AppUser { Id = inviteeId, Email = "invitee@test.com" };
        _userRepoMock.Setup(r => r.GetByIdsAsync(inviteesIds))
            .ReturnsAsync(new List<AppUser> { invitee });

        // Act
        var result = await _bookingService.CreateInvitationsAsync(meetingId, inviteesIds, inviterId);

        // Assert
        Assert.Empty(result); // Приглашение не создано, т.к. уже существует
    }

    #endregion

    #region RespondToInvitationAsync Tests

    [Fact]
    public async Task RespondToInvitationAsync_InvitationNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync((MeetingInvitation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _bookingService.RespondToInvitationAsync(invitationId, userId, true));
    }

    [Fact]
    public async Task RespondToInvitationAsync_WrongUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();

        var invitee = new AppUser { Id = userId, Email = "invitee@test.com" };
        var inviter = new AppUser { Id = Guid.NewGuid(), Email = "inviter@test.com" };
        var meeting = new Meeting(inviter.Id, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        var invitation = new MeetingInvitation(meeting.Id, userId, inviter.Id);
        
        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync(invitation);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _bookingService.RespondToInvitationAsync(invitationId, wrongUserId, true));
    }

    [Fact]
    public async Task RespondToInvitationAsync_Accept_Success()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var invitee = new AppUser { Id = userId, Email = "invitee@test.com" };
        var inviter = new AppUser { Id = Guid.NewGuid(), Email = "inviter@test.com" };
        var meeting = new Meeting(inviter.Id, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        var invitation = new MeetingInvitation(meeting.Id, userId, inviter.Id);
        
        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync(invitation);

        _invitationRepoMock.Setup(r => r.UpdateAsync(invitation))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _bookingService.RespondToInvitationAsync(invitationId, userId, true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Accepted", result.Status);
        Assert.NotNull(result.RespondedAt);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RespondToInvitationAsync_Decline_Success()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var invitee = new AppUser { Id = userId, Email = "invitee@test.com" };
        var inviter = new AppUser { Id = Guid.NewGuid(), Email = "inviter@test.com" };
        var meeting = new Meeting(inviter.Id, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        var invitation = new MeetingInvitation(meeting.Id, userId, inviter.Id);
        
        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync(invitation);

        _invitationRepoMock.Setup(r => r.UpdateAsync(invitation))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _bookingService.RespondToInvitationAsync(invitationId, userId, false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Declined", result.Status);
        Assert.NotNull(result.RespondedAt);
    }

    [Fact]
    public async Task RespondToInvitationAsync_AlreadyResponded_ThrowsInvalidOperationException()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var invitee = new AppUser { Id = userId, Email = "invitee@test.com" };
        var inviter = new AppUser { Id = Guid.NewGuid(), Email = "inviter@test.com" };
        var meeting = new Meeting(inviter.Id, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        var invitation = new MeetingInvitation(meeting.Id, userId, inviter.Id);
        invitation.Accept(); // Уже принято
        
        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync(invitation);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _bookingService.RespondToInvitationAsync(invitationId, userId, false));
    }

    #endregion

    #region CancelInvitationAsync Tests

    [Fact]
    public async Task CancelInvitationAsync_InvitationNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync((MeetingInvitation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _bookingService.CancelInvitationAsync(invitationId, userId));
    }

    [Fact]
    public async Task CancelInvitationAsync_NotInviterOrInvitee_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();

        var inviter = new AppUser { Id = inviterId, Email = "inviter@test.com" };
        var invitee = new AppUser { Id = inviteeId, Email = "invitee@test.com" };
        var meeting = new Meeting(inviterId, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        var invitation = new MeetingInvitation(meeting.Id, inviteeId, inviterId);
        
        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync(invitation);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _bookingService.CancelInvitationAsync(invitationId, userId));
    }

    [Fact]
    public async Task CancelInvitationAsync_ByInviter_Success()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();

        var inviter = new AppUser { Id = inviterId, Email = "inviter@test.com" };
        var invitee = new AppUser { Id = inviteeId, Email = "invitee@test.com" };
        var meeting = new Meeting(inviterId, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        var invitation = new MeetingInvitation(meeting.Id, inviteeId, inviterId);
        
        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync(invitation);

        _invitationRepoMock.Setup(r => r.UpdateAsync(invitation))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _bookingService.CancelInvitationAsync(invitationId, inviterId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Declined", result.Status);
        Assert.NotNull(result.RespondedAt);
    }

    [Fact]
    public async Task CancelInvitationAsync_ByInvitee_Success()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var inviteeId = Guid.NewGuid();

        var inviter = new AppUser { Id = inviterId, Email = "inviter@test.com" };
        var invitee = new AppUser { Id = inviteeId, Email = "invitee@test.com" };
        var meeting = new Meeting(inviterId, Guid.NewGuid(), "Test", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        
        var invitation = new MeetingInvitation(meeting.Id, inviteeId, inviterId);
        
        _invitationRepoMock.Setup(r => r.GetByIdAsyncWithInclude(invitationId))
            .ReturnsAsync(invitation);

        _invitationRepoMock.Setup(r => r.UpdateAsync(invitation))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _bookingService.CancelInvitationAsync(invitationId, inviteeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Declined", result.Status);
    }

    #endregion

    #region GetInvitationsForUserAsync Tests

    [Fact]
    public async Task GetInvitationsForUserAsync_ReturnsInvitations()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();
        var meetingId = Guid.NewGuid();

        var invitations = new List<MeetingInvitation>
        {
            new MeetingInvitation(meetingId, userId, inviterId),
            new MeetingInvitation(meetingId, userId, inviterId)
        };

        _invitationRepoMock.Setup(r => r.GetByInviteeIdAsyncWithInclude(userId))
            .ReturnsAsync(invitations);

        // Act
        var result = await _bookingService.GetInvitationsForUserAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetInvitationsForUserAsync_NoInvitations_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _invitationRepoMock.Setup(r => r.GetByInviteeIdAsyncWithInclude(userId))
            .ReturnsAsync(new List<MeetingInvitation>());

        // Act
        var result = await _bookingService.GetInvitationsForUserAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetInvitationsForMeetingAsync Tests

    [Fact]
    public async Task GetInvitationsForMeetingAsync_ReturnsInvitations()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var inviterId = Guid.NewGuid();

        var invitations = new List<MeetingInvitation>
        {
            new MeetingInvitation(meetingId, userId, inviterId),
            new MeetingInvitation(meetingId, userId, inviterId),
            new MeetingInvitation(meetingId, Guid.NewGuid(), inviterId)
        };

        _invitationRepoMock.Setup(r => r.GetByMeetingIdAsyncWithInclude(meetingId))
            .ReturnsAsync(invitations);

        // Act
        var result = await _bookingService.GetInvitationsForMeetingAsync(meetingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetInvitationsForMeetingAsync_NoInvitations_ReturnsEmptyList()
    {
        // Arrange
        var meetingId = Guid.NewGuid();

        _invitationRepoMock.Setup(r => r.GetByMeetingIdAsyncWithInclude(meetingId))
            .ReturnsAsync(new List<MeetingInvitation>());

        // Act
        var result = await _bookingService.GetInvitationsForMeetingAsync(meetingId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}
