using BookingSystem.Application.DTOs;
using BookingSystem.Application.Exceptions;
using BookingSystem.Core.Entities;
using BookingSystem.Core.Entities.Aggregates;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IUserRepository _userRepo;
    private readonly IInvitationRepository _invitationRepo;
    private readonly INotificationService _notificationService;
    private readonly ILogger<BookingService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo, IUserRepository userRepo, IInvitationRepository invitationRepo, INotificationService notificationService, ILogger<BookingService> logger, IUnitOfWork unitOfWork)
    {
        _bookingRepo = bookingRepo;
        _roomRepo = roomRepo;
        _userRepo = userRepo;
        _invitationRepo = invitationRepo;
        _notificationService = notificationService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingDto> CreateBookingAsync(CreateBookingRequest request)
    {
        _logger.LogInformation("Attempting to create an meeting in Room {@RoomId}, creator Id is {@UserId}, Time range: {@StartTime} -- {@EndTime}", request.RoomId, request.UserId, request.StartTime, request.EndTime);
        var room = await _roomRepo.GetByIdAsync(request.RoomId);
        if (room == null) throw new KeyNotFoundException($"Room with ID {request.RoomId} not found.");

        var creator = await _userRepo.GetByIdAsync(request.UserId);
        if (creator == null) throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        // Проверка конфликта по времени
        var existingBookings = await _bookingRepo.GetByRoomIdAsync(request.RoomId, request.StartTime, request.EndTime);
        if (existingBookings.Any(b => !b.IsCancelled))
        {
            throw new BookingConflictException("Room is already booked during this time.");
        }
        List<AppUser> Subscribers = new();
        //подписка всех указанных подписчиков
        if (request.SubscribersId.Any())
        {

            IEnumerable<AppUser> users = await _userRepo.GetByIdsAsync(request.SubscribersId);
            //если кто то не нашелся - эксепшен
            if (users.Count() != request.SubscribersId.Count())
            {
                var foundIds = users.Select(r => r.Id);
                var notFoundIds = request.SubscribersId.Except(foundIds);
                throw new ArgumentException($"One or more users not found. Invalid IDs: {string.Join(", ", notFoundIds)}");
            }
            foreach (var item in users)
            {
                Subscribers.Add(item);
            }
        }


        var meeting = new Meeting(creator.Id,
            room.Id, request.Purpose, request.StartTime, request.EndTime);

        await _bookingRepo.AddAsync(meeting);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Succesfully created an meeting with Id {@MeetingId}", meeting.Id );

        // Отправка уведомлений о создании встречи
        await _notificationService.SendMeetingCreatedEmailAsync(meeting.Id, creator.Id);
        
        // Уведомления для всех подписчиков
        foreach (var subscriber in Subscribers)
        {
            await _notificationService.SendMeetingCreatedEmailAsync(meeting.Id, subscriber.Id);
        }

        return MapToDto(meeting);
    }
    public async Task<BookingDto> UpdateBookingAsync(UpdateBookingRequest request)
    {
        _logger.LogInformation("Attempting to update an meeting {@BookingId}, Time range: {@StartTime} -- {@EndTime}", request.BookingId, request.StartTime, request.EndTime);

        var booking = await _bookingRepo.GetByIdAsyncWithInclude(request.BookingId);
        if (booking == null) throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");

        
        var existingBookings = await _bookingRepo.GetByRoomIdAsync(booking.Room.Id, request.StartTime, request.EndTime);
        if (existingBookings.Any(b => b.Id != request.BookingId && !b.IsCancelled))
        {
            throw new BookingConflictException("Room is already booked during this time.");
        }

        booking.UpdateTimeRange(request.StartTime, request.EndTime);

            IEnumerable<AppUser> newUsers = await _userRepo.GetByIdsAsync(request.SubscribersId);
     
            //если кто то не нашелся - эксепшен
            if (newUsers.Count() != request.SubscribersId.Count())
            {
                var foundIds = newUsers.Select(r => r.Id);
                var notFoundIds = request.SubscribersId.Except(foundIds);
                throw new ArgumentException($"One or more users not found. Invalid IDs: {string.Join(", ", notFoundIds)}");
            }


                var newbie = newUsers.Where(u => !booking.Subscribers.Contains(u)).ToList();
                var unsubs = booking.Subscribers.Where(u => !newUsers.Contains(u)).ToList();
            foreach (var item in newbie)
            {
                booking.Subscribe(item);
            }
            foreach (var item in unsubs)
            {
                booking.Unsubscribe(item);
            }
        
        await _bookingRepo.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Succesfully update an meeting {@BookingId}, Time range: {@StartTime} -- {@EndTime}", request.BookingId, request.StartTime, request.EndTime);

        return MapToDto(booking);
    }

    public async Task CancelBookingAsync(Guid bookingId)
    {
        _logger.LogInformation("Attempting to cancel an meeting {@BookingId}", bookingId);

        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null) throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");

        booking.Cancel();
        
        await _bookingRepo.UpdateAsync(booking);
        _logger.LogInformation("Succesfully cancel an meeting {@BookingId}", bookingId);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId)
    {
        _logger.LogInformation("Trying to find an meeting {@BookingId}", bookingId);

        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null) throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
        _logger.LogInformation("Succesfully find an meeting {@BookingId}", bookingId);

        return MapToDto(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByRoomIdAsync(Guid roomId)
    {
        _logger.LogInformation("Trying to find an meetings by room {@RoomId}", roomId);
        var bookings = await _bookingRepo.GetByRoomIdAsync(roomId, DateTime.MinValue, DateTime.MaxValue);
        return bookings.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(Guid userId)
    {
        _logger.LogInformation("Trying to find an meetings by room {@RoomId}", userId);
        var bookings = await _bookingRepo.GetByUserIdAsync(userId);
        return bookings.Select(MapToDto);
    }

    public async Task<List<MeetingInvitationDto>> CreateInvitationsAsync(Guid meetingId, List<Guid> inviteesIds, Guid inviterId)
    {
        _logger.LogInformation("Attempting to create invitations for meeting {@MeetingId} for users {@InviteesIds}", meetingId, inviteesIds);

        var meeting = await _bookingRepo.GetByIdAsyncWithInclude(meetingId);
        if (meeting == null) throw new KeyNotFoundException($"Meeting with ID {meetingId} not found.");

        var inviter = await _userRepo.GetByIdAsync(inviterId);
        if (inviter == null) throw new KeyNotFoundException($"Inviter with ID {inviterId} not found.");

        var invitees = await _userRepo.GetByIdsAsync(inviteesIds);
        if (invitees.Count() != inviteesIds.Count)
        {
            var foundIds = invitees.Select(u => u.Id);
            var notFoundIds = inviteesIds.Except(foundIds);
            throw new ArgumentException($"One or more users not found. Invalid IDs: {string.Join(", ", notFoundIds)}");
        }

        var invitations = new List<MeetingInvitation>();
        foreach (var invitee in invitees)
        {
            try
            {
                var invitation = meeting.CreateInvitation(invitee.Id, inviterId);
                invitations.Add(invitation);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Skipping invitation for user {@UserId}: {Message}", invitee.Id, ex.Message);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Successfully created {Count} invitations for meeting {@MeetingId}", invitations.Count, meetingId);

        // Отправка уведомлений о приглашении каждому приглашенному
        foreach (var invitation in invitations)
        {
            await _notificationService.SendMeetingInvitationEmailAsync(invitation.Id);
        }

        return invitations.Select(MapToDto).ToList();
    }

    public async Task<MeetingInvitationDto> RespondToInvitationAsync(Guid invitationId, Guid userId, bool accept)
    {
        _logger.LogInformation("User {@UserId} responding to invitation {@InvitationId} with accept={Accept}", userId, invitationId, accept);

        var invitation = await _invitationRepo.GetByIdAsyncWithInclude(invitationId);
        if (invitation == null) throw new KeyNotFoundException($"Invitation with ID {invitationId} not found.");

        if (invitation.InviteeId != userId)
            throw new UnauthorizedAccessException("Only the invitee can respond to this invitation.");

        if (accept)
            invitation.Accept();
        else
            invitation.Decline();

        await _invitationRepo.UpdateAsync(invitation);
        await _unitOfWork.SaveChangesAsync();

        // Если приглашение отклонено - отправляем уведомление организатору
        if (!accept)
        {
            await _notificationService.SendInvitationDeclinedEmailAsync(invitationId);
        }

        _logger.LogInformation("User {@UserId} responded to invitation {@InvitationId} with accept={Accept}", userId, invitationId, accept);

        return MapToDto(invitation);
    }

    public async Task<MeetingInvitationDto> CancelInvitationAsync(Guid invitationId, Guid userId)
    {
        _logger.LogInformation("User {@UserId} cancelling invitation {@InvitationId}", userId, invitationId);

        var invitation = await _invitationRepo.GetByIdAsyncWithInclude(invitationId);
        if (invitation == null) throw new KeyNotFoundException($"Invitation with ID {invitationId} not found.");

        if (invitation.InviterId != userId && invitation.InviteeId != userId)
            throw new UnauthorizedAccessException("Only the inviter or invitee can cancel this invitation.");

        invitation.Cancel();

        await _invitationRepo.UpdateAsync(invitation);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {@UserId} cancelled invitation {@InvitationId}", userId, invitationId);

        return MapToDto(invitation);
    }

    public async Task<List<MeetingInvitationDto>> GetInvitationsForUserAsync(Guid userId)
    {
        _logger.LogInformation("Getting invitations for user {@UserId}", userId);

        var invitations = await _invitationRepo.GetByInviteeIdAsyncWithInclude(userId);
        return invitations.Select(MapToDto).ToList();
    }

    public async Task<List<MeetingInvitationDto>> GetInvitationsForMeetingAsync(Guid meetingId)
    {
        _logger.LogInformation("Getting invitations for meeting {@MeetingId}", meetingId);

        var invitations = await _invitationRepo.GetByMeetingIdAsyncWithInclude(meetingId);
        return invitations.Select(MapToDto).ToList();
    }

    private static BookingDto MapToDto(Meeting meeting)
    {
        return new()
        {
            Id = meeting.Id,
            RoomId = meeting.RoomId,
            CreatorId = meeting.CreatorId,
            StartTime = meeting.TimeRange.Start,
            EndTime = meeting.TimeRange.End,
            IsCancelled = meeting.IsCancelled,
            Purpose = meeting.Reason,
            SubscribersNames = meeting.Subscribers.Select(s => s.Name ?? string.Empty).ToList(),
            Invitations = meeting.Invitations.Select(MapToDto).ToList()
        };
    }

    private static MeetingInvitationDto MapToDto(MeetingInvitation invitation)
    {
        return new()
        {
            Id = invitation.Id,
            MeetingId = invitation.MeetingId,
            InviteeId = invitation.InviteeId,
            InviteeName = invitation.Invitee?.Name ?? string.Empty,
            InviterId = invitation.InviterId,
            InviterName = invitation.Inviter?.Name ?? string.Empty,
            Status = invitation.Status.ToString(),
            CreatedAt = invitation.CreatedAt,
            RespondedAt = invitation.RespondedAt
        };
    }
}

