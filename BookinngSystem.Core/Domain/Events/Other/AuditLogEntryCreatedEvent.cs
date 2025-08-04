namespace BookingSystem.Core.Domain.Events
{
    public class AuditLogEntryCreatedEvent : IDomainEvent
    {
        public Guid AuditLogId { get; }
        public string Action { get; }
        public Guid EntityId { get; }
        public string EntityType { get; }
        public string OldData { get; }
        public string NewData { get; }
        public DateTime OccurredOn { get; }

        public AuditLogEntryCreatedEvent(
             Guid auditLogId,
             string action,
            string entityType,
            Guid entityId,
            string oldData,
            string newData)
        {
            AuditLogId = auditLogId;
            Action = action;
            EntityType = entityType;
            EntityId = entityId;
            OldData = oldData;
            NewData = newData;
            OccurredOn = DateTime.UtcNow;
        }
    }
}