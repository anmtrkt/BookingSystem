using BookingSystem.Core.Domain.Events;

namespace BookingSystem.Core.Domain.Events
{
    public class BookingOverriddenEvent : IDomainEvent
    {
        public Guid OldMeetingId { get; }
        public Guid NewMeetingId { get; }
        public Guid OverriderInstitutionId { get; }
        public Guid OverridedInstitutionId { get; }
        public DateTime OccurredOn { get; }

        public BookingOverriddenEvent(
            Guid oldMeetingId,
            Guid newMeetingId,
            Guid overriderInstitutionId,
            Guid overridedInstitutionId)
        {
            OldMeetingId = oldMeetingId;
            NewMeetingId = newMeetingId;
            OverriderInstitutionId = overriderInstitutionId;
            OverridedInstitutionId = overridedInstitutionId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}