using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Events.UserEvents
{
    class UserUpdateInstitutionEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public Guid OldInstitutionId { get; }
        public  Guid NewInstitutionId { get; }
        public DateTime OccurredOn { get; }

        public UserUpdateInstitutionEvent(
             Guid userId,
            Guid oldInstitutionId, 
            Guid newInstitutionId)
        {
            UserId = userId;
            OldInstitutionId = oldInstitutionId;
            NewInstitutionId = newInstitutionId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
