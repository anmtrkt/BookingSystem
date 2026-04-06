using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;
public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingRequest request);
    Task<BookingDto> UpdateBookingAsync(UpdateBookingRequest request);
    Task CancelBookingAsync(Guid bookingId);
    Task<BookingDto> GetBookingByIdAsync(Guid id);
    Task<IEnumerable<BookingDto>> GetBookingsByRoomIdAsync(Guid roomId);
    Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(Guid userId);
    Task<List<MeetingInvitationDto>> CreateInvitationsAsync(Guid meetingId, List<Guid> inviteesIds, Guid inviterId);
    Task<MeetingInvitationDto> RespondToInvitationAsync(Guid invitationId, Guid userId, bool accept);
    Task<MeetingInvitationDto> CancelInvitationAsync(Guid invitationId, Guid userId);
    Task<List<MeetingInvitationDto>> GetInvitationsForUserAsync(Guid userId);
    Task<List<MeetingInvitationDto>> GetInvitationsForMeetingAsync(Guid meetingId);
}