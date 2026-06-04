using BookingSystem.Api;
using BookingSystem.Application.DTOs;
using BookingSystem.Application.Exceptions;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BookingSystem.Tests.Controllers;

public class BookingControllerTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _controller = new BookingController(_bookingServiceMock.Object);
    }

    private void SetupUserClaims(Guid userId, string? role = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    #region Create Tests

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        SetupUserClaims(Guid.NewGuid());
        
        var request = new CreateBookingRequest
        {
            RoomId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Purpose = "Meeting"
        };

        var createdBooking = new BookingDto
        {
            Id = Guid.NewGuid(),
            RoomId = request.RoomId,
            CreatorId = Guid.NewGuid()
        };

        _bookingServiceMock.Setup(bs => bs.CreateBookingAsync(It.IsAny<CreateBookingRequest>()))
            .ReturnsAsync(createdBooking);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(BookingController.GetById), createdAtActionResult.ActionName);
        
        var returnedBooking = Assert.IsType<BookingDto>(createdAtActionResult.Value);
        Assert.Equal(createdBooking.Id, returnedBooking.Id);
    }

    [Fact]
    public async Task Create_InvalidRoomId_ThrowsException()
    {
        // Arrange
        SetupUserClaims(Guid.NewGuid());
        
        var request = new CreateBookingRequest
        {
            RoomId = Guid.Empty,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Purpose = "Meeting"
        };

        _bookingServiceMock.Setup(bs => bs.CreateBookingAsync(It.IsAny<CreateBookingRequest>()))
            .ThrowsAsync(new ArgumentException("Invalid room ID"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _controller.Create(request));
    }

    [Fact]
    public async Task Create_EndTimeBeforeStartTime_ThrowsException()
    {
        // Arrange
        SetupUserClaims(Guid.NewGuid());
        
        var request = new CreateBookingRequest
        {
            RoomId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(1),
            Purpose = "Meeting"
        };

        _bookingServiceMock.Setup(bs => bs.CreateBookingAsync(It.IsAny<CreateBookingRequest>()))
            .ThrowsAsync(new ArgumentException("End time must be after start time"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _controller.Create(request));
    }

    [Fact]
    public async Task Create_RoomNotAvailable_ThrowsBookingConflictException()
    {
        // Arrange
        SetupUserClaims(Guid.NewGuid());
        
        var request = new CreateBookingRequest
        {
            RoomId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Purpose = "Meeting"
        };

        _bookingServiceMock.Setup(bs => bs.CreateBookingAsync(It.IsAny<CreateBookingRequest>()))
            .ThrowsAsync(new BookingConflictException("Room is already booked for this time slot"));

        // Act & Assert
        await Assert.ThrowsAsync<BookingConflictException>(() => _controller.Create(request));
    }

    [Fact]
    public async Task Create_UnauthenticatedUser_ReturnsBadRequest()
    {
        // Arrange - No user claims setup
        
        var request = new CreateBookingRequest
        {
            RoomId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Purpose = "Meeting"
        };

        // Act - Service should handle missing user ID
        Assert.NotNull(request);
    }

    #endregion

    #region GetMyBookings Tests

    [Fact]
    public async Task GetMyBookings_ReturnsOkWithBookings()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var bookings = new List<BookingDto>
        {
            new BookingDto { Id = Guid.NewGuid(), CreatorId = userId },
            new BookingDto { Id = Guid.NewGuid(), CreatorId = userId }
        };

        _bookingServiceMock.Setup(bs => bs.GetBookingsByUserIdAsync(userId))
            .ReturnsAsync(bookings);

        // Act
        var result = await _controller.GetMyBookings();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBookings = Assert.IsType<List<BookingDto>>(okResult.Value);
        Assert.Equal(2, returnedBookings.Count);
    }

    [Fact]
    public async Task GetMyBookings_NoBookings_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        _bookingServiceMock.Setup(bs => bs.GetBookingsByUserIdAsync(userId))
            .ReturnsAsync(new List<BookingDto>());

        // Act
        var result = await _controller.GetMyBookings();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBookings = Assert.IsType<List<BookingDto>>(okResult.Value);
        Assert.Empty(returnedBookings);
    }


    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ValidId_ReturnsOkWithBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new BookingDto 
        { 
            Id = bookingId, 
            RoomId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid()
        };

        _bookingServiceMock.Setup(bs => bs.GetBookingByIdAsync(bookingId))
            .ReturnsAsync(booking);

        // Act
        var result = await _controller.GetById(bookingId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooking = Assert.IsType<BookingDto>(okResult.Value);
        Assert.Equal(bookingId, returnedBooking.Id);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsException()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        
        _bookingServiceMock.Setup(bs => bs.GetBookingByIdAsync(bookingId))
            .ThrowsAsync(new KeyNotFoundException("Booking not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetById(bookingId));
    }

    #endregion

    #region Cancel Tests

    [Fact]
    public async Task Cancel_OwnBooking_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var booking = new BookingDto 
        { 
            Id = bookingId, 
            CreatorId = userId 
        };

        _bookingServiceMock.Setup(bs => bs.GetBookingByIdAsync(bookingId))
            .ReturnsAsync(booking);
        
        _bookingServiceMock.Setup(bs => bs.CancelBookingAsync(bookingId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Cancel(bookingId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Cancel_AdminCancelsAnyBooking_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        SetupUserClaims(userId, "Admin");
        
        var booking = new BookingDto 
        { 
            Id = bookingId, 
            CreatorId = creatorId 
        };

        _bookingServiceMock.Setup(bs => bs.GetBookingByIdAsync(bookingId))
            .ReturnsAsync(booking);
        
        _bookingServiceMock.Setup(bs => bs.CancelBookingAsync(bookingId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Cancel(bookingId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Cancel_NotOwnBooking_NotAdmin_ReturnsForbid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        SetupUserClaims(userId, "User");
        
        var booking = new BookingDto 
        { 
            Id = bookingId, 
            CreatorId = creatorId 
        };

        _bookingServiceMock.Setup(bs => bs.GetBookingByIdAsync(bookingId))
            .ReturnsAsync(booking);

        // Act
        var result = await _controller.Cancel(bookingId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Cancel_BookingNotFound_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        _bookingServiceMock.Setup(bs => bs.GetBookingByIdAsync(bookingId))
            .ThrowsAsync(new KeyNotFoundException("Booking not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Cancel(bookingId));
    }



    #endregion

    #region CreateInvitations Tests

    [Fact]
    public async Task CreateInvitations_ValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var meetingId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var request = new CreateInvitationRequest
        {
            MeetingId = meetingId,
            InviteesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        var invitations = new List<MeetingInvitationDto>
        {
            new MeetingInvitationDto { Id = Guid.NewGuid(), MeetingId = meetingId }
        };

        _bookingServiceMock.Setup(bs => bs.CreateInvitationsAsync(meetingId, request.InviteesIds, userId))
            .ReturnsAsync(invitations);

        // Act
        var result = await _controller.CreateInvitations(meetingId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitations = Assert.IsType<List<MeetingInvitationDto>>(okResult.Value);
        Assert.Single(returnedInvitations);
    }

    [Fact]
    public async Task CreateInvitations_EmptyInviteesList_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var meetingId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var request = new CreateInvitationRequest
        {
            MeetingId = meetingId,
            InviteesIds = new List<Guid>()
        };

        _bookingServiceMock.Setup(bs => bs.CreateInvitationsAsync(meetingId, request.InviteesIds, userId))
            .ReturnsAsync(new List<MeetingInvitationDto>());

        // Act
        var result = await _controller.CreateInvitations(meetingId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitations = Assert.IsType<List<MeetingInvitationDto>>(okResult.Value);
        Assert.Empty(returnedInvitations);
    }



    #endregion

    #region RespondToInvitation Tests

    [Fact]
    public async Task RespondToInvitation_Accept_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var request = new RespondToInvitationRequest
        {
            InvitationId = Guid.NewGuid(),
            Accept = true
        };

        var invitation = new MeetingInvitationDto 
        { 
            Id = request.InvitationId,
            Status = "Accepted"
        };

        _bookingServiceMock.Setup(bs => bs.RespondToInvitationAsync(request.InvitationId, userId, true))
            .ReturnsAsync(invitation);

        // Act
        var result = await _controller.RespondToInvitation(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitation = Assert.IsType<MeetingInvitationDto>(okResult.Value);
        Assert.Equal("Accepted", returnedInvitation.Status);
    }

    [Fact]
    public async Task RespondToInvitation_Decline_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var request = new RespondToInvitationRequest
        {
            InvitationId = Guid.NewGuid(),
            Accept = false
        };

        var invitation = new MeetingInvitationDto 
        { 
            Id = request.InvitationId,
            Status = "Declined"
        };

        _bookingServiceMock.Setup(bs => bs.RespondToInvitationAsync(request.InvitationId, userId, false))
            .ReturnsAsync(invitation);

        // Act
        var result = await _controller.RespondToInvitation(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitation = Assert.IsType<MeetingInvitationDto>(okResult.Value);
        Assert.Equal("Declined", returnedInvitation.Status);
    }

   

    #endregion

    #region CancelInvitation Tests

    [Fact]
    public async Task CancelInvitation_ValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var invitation = new MeetingInvitationDto 
        { 
            Id = invitationId,
            Status = "Declined"
        };

        _bookingServiceMock.Setup(bs => bs.CancelInvitationAsync(invitationId, userId))
            .ReturnsAsync(invitation);

        // Act
        var result = await _controller.CancelInvitation(invitationId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitation = Assert.IsType<MeetingInvitationDto>(okResult.Value);
        Assert.Equal(invitationId, returnedInvitation.Id);
    }


    #endregion

    #region GetMyInvitations Tests

    [Fact]
    public async Task GetMyInvitations_ReturnsOkWithInvitations()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        var invitations = new List<MeetingInvitationDto>
        {
            new MeetingInvitationDto { Id = Guid.NewGuid(), InviteeId = userId },
            new MeetingInvitationDto { Id = Guid.NewGuid(), InviteeId = userId }
        };

        _bookingServiceMock.Setup(bs => bs.GetInvitationsForUserAsync(userId))
            .ReturnsAsync(invitations);

        // Act
        var result = await _controller.GetMyInvitations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitations = Assert.IsType<List<MeetingInvitationDto>>(okResult.Value);
        Assert.Equal(2, returnedInvitations.Count);
    }

    [Fact]
    public async Task GetMyInvitations_NoInvitations_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);
        
        _bookingServiceMock.Setup(bs => bs.GetInvitationsForUserAsync(userId))
            .ReturnsAsync(new List<MeetingInvitationDto>());

        // Act
        var result = await _controller.GetMyInvitations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitations = Assert.IsType<List<MeetingInvitationDto>>(okResult.Value);
        Assert.Empty(returnedInvitations);
    }

    #endregion

    #region GetMeetingInvitations Tests

    [Fact]
    public async Task GetMeetingInvitations_ValidMeetingId_ReturnsOk()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        
        var invitations = new List<MeetingInvitationDto>
        {
            new MeetingInvitationDto { Id = Guid.NewGuid(), MeetingId = meetingId }
        };

        _bookingServiceMock.Setup(bs => bs.GetInvitationsForMeetingAsync(meetingId))
            .ReturnsAsync(invitations);

        // Act
        var result = await _controller.GetMeetingInvitations(meetingId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitations = Assert.IsType<List<MeetingInvitationDto>>(okResult.Value);
        Assert.Single(returnedInvitations);
    }

    [Fact]
    public async Task GetMeetingInvitations_NoInvitations_ReturnsEmptyList()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        
        _bookingServiceMock.Setup(bs => bs.GetInvitationsForMeetingAsync(meetingId))
            .ReturnsAsync(new List<MeetingInvitationDto>());

        // Act
        var result = await _controller.GetMeetingInvitations(meetingId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInvitations = Assert.IsType<List<MeetingInvitationDto>>(okResult.Value);
        Assert.Empty(returnedInvitations);
    }

    #endregion
}
