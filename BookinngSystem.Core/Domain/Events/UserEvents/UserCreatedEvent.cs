using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Events.UserEvents
{
    public class UserCreatedEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public DateTime OccurredOn { get; }
        public UserCreatedEvent(
            Guid userId
            )
        {
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
        }

    }
}
