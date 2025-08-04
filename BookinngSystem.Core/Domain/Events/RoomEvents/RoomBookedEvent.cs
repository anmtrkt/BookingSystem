using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Events
{
    public class RoomBookedEvent : IDomainEvent
    {
        public Guid RoomId { get; }
        public DateTime BookingTime { get; }
        public Guid InstitutionId { get; }
        public byte InstitutionPriority { get; }
        public DateTime OccurredOn { get; }

        public RoomBookedEvent(
            Guid roomId,
            Guid institutionId,
            DateTime bookingTime,
            byte institutionPriority)
        {
            RoomId = roomId;
            InstitutionId = institutionId;
            BookingTime = bookingTime;
            InstitutionPriority = institutionPriority;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
