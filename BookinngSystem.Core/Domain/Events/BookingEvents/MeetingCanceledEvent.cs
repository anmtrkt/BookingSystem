using BookingSystem.Core.Domain.Events;

namespace BookingSystem.Core.Domain.Events
{
    public class MeetingCanceledEvent : IDomainEvent
    {
        public Guid MeetingId { get; }
        public Guid UserId { get; }
        public string Reason { get; }
        public DateTime OccurredOn { get; }

        public MeetingCanceledEvent(
             Guid meetingId,
                Guid userId,
             string reason)
        {
            MeetingId = meetingId;
            UserId = userId;
            Reason = reason;
            OccurredOn = DateTime.UtcNow;
        }
    }
}