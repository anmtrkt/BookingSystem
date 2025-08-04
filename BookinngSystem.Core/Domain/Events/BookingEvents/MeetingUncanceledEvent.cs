using BookingSystem.Core.Domain.Events;

namespace BookingSystem.Core.Domain.Events
{
    public class MeetingUncanceledEvent : IDomainEvent
    {
        public Guid MeetingId { get; }
        public Guid UserId { get; }
        public DateTime OccurredOn { get; }

        public MeetingUncanceledEvent(
             Guid meetingId,
            Guid userId)
        {
            MeetingId = meetingId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}