using BookingSystem.Core.Domain.Events;
namespace BookingSystem.Core.Domain.Events
{
    public class MeetingCreatedEvent : IDomainEvent
    {
        public Guid MeetingId { get; }
        public Guid UserId { get; }
        public Guid RoomId { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public DateTime OccurredOn { get; }

        public MeetingCreatedEvent(
             Guid meetingId,
            Guid userId,
            Guid roomId,
            DateTime startTime,
            DateTime endTime)
        {
            MeetingId = meetingId;
            UserId = userId;
            RoomId = roomId;
            StartTime = startTime;
            EndTime = endTime;
            OccurredOn = DateTime.UtcNow;
        }
    }
}