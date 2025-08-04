using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Entities
{
    public class UserSubscribeRequest
    {
        public Guid UserId { get; set; }
        public Guid MeetingId { get; set; }
        public UserSubscribeRequest() { }

    }
}
