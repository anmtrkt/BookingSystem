namespace BookingSystem.Core.Domain.Events
{
    public class UserRoleAssignedEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public Guid RoleId { get; }
        public DateTime EffectiveDate { get; }
        public DateTime OccurredOn { get; }

        public UserRoleAssignedEvent(
             Guid userId,
             Guid roleId,
             DateTime effectiveDate)
        {
            UserId = userId;
            RoleId = roleId;
            EffectiveDate = effectiveDate;
            OccurredOn = DateTime.UtcNow;
        }
    }
}