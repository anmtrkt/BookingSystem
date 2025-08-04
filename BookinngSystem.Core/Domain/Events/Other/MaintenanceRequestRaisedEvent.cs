namespace BookingSystem.Core.Domain.Events
{
    public class MaintenanceRequestRaisedEvent : IDomainEvent
    {
        public Guid RequestId { get; }
        public Guid RoomId { get; }
        public Guid EquipmentId { get; }
        public string Description { get; }
        public DateTime OccurredOn { get; }

        public MaintenanceRequestRaisedEvent(
            Guid requestId,
            Guid roomId,
            Guid equipmentId,
            string description)
        {
            RequestId = requestId;
            RoomId = roomId;
            EquipmentId = equipmentId;
            Description = description;
            OccurredOn = DateTime.UtcNow;
        }
    }
}