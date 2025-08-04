using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Events.RoomEvents
{
    class RoomOwnerChangedEvent : IDomainEvent
    {


        public Guid RoomId { get; }
        public Guid InstitutionId { get; }
        public DateTime OccurredOn { get; }

        public RoomOwnerChangedEvent(
            Guid roomId,
            Guid institutionId)
        {
            RoomId = roomId;
            InstitutionId = institutionId;
            OccurredOn = DateTime.UtcNow;
        }
    }

}
