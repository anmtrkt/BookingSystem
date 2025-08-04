using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Events.UserEvents
{
    public class UserInfoUpdateEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public UserInfoUpdateEvent(Guid Id, string Post, string LastPost)
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}
