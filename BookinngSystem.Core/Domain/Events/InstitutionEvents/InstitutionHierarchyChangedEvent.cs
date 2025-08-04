namespace BookingSystem.Core.Domain.Events
{
    public class InstitutionHierarchyChangedEvent : IDomainEvent
    {
        public Guid InstitutionId { get; }
        public Guid? ParentId { get; }
        public string ActionType { get; } // "Added" or "Removed"
        public DateTime OccurredOn { get; }

        public InstitutionHierarchyChangedEvent(
             Guid institutionId,
            Guid? parentId,
            string actionType)
        {
            InstitutionId = institutionId;
            ParentId = parentId;
            ActionType = actionType;
            OccurredOn = DateTime.UtcNow;
        }
    }
}