using BookingSystem.Core.Domain.ValueObjects;

namespace BookingSystem.Core.Domain.Events
{
    public class RoomEquipmentUpdatedEvent : IDomainEvent
    {
        public Guid RoomId { get; }
        public Equipment OldEquipment { get; }
        public Equipment NewEquipment { get; }
        public DateTime OccurredOn { get; }

        public RoomEquipmentUpdatedEvent(
             Guid roomId,
            Equipment oldEquipment,
            Equipment newEquipment)
        {
            RoomId = roomId;
            OldEquipment = oldEquipment;
            NewEquipment = newEquipment;
            OccurredOn = DateTime.UtcNow;
        }
    }
}