using BookingSystem.Core.Entities.Aggregates;

namespace BookingSystem.Infrastructure.Repositories;


public interface IInvitationRepository
{
    Task<MeetingInvitation?> GetByIdAsync(Guid id);
    Task<MeetingInvitation?> GetByIdAsyncWithInclude(Guid id);
    Task<IEnumerable<MeetingInvitation>> GetByMeetingIdAsync(Guid meetingId);
    Task<IEnumerable<MeetingInvitation>> GetByMeetingIdAsyncWithInclude(Guid meetingId);
    Task<IEnumerable<MeetingInvitation>> GetByInviteeIdAsync(Guid inviteeId);
    Task<IEnumerable<MeetingInvitation>> GetByInviteeIdAsyncWithInclude(Guid inviteeId);
    Task<IEnumerable<MeetingInvitation>> GetByInviterIdAsync(Guid inviterId);
    Task AddAsync(MeetingInvitation invitation);
    Task UpdateAsync(MeetingInvitation invitation);
    Task DeleteAsync(Guid id);
}
