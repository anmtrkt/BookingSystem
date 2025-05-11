using BookingSystem.Core.Domain.Common;

namespace BookingSystem.Core.Domain.Entities
{
    class Official : BaseEntity
    {

        public Institution Institution { get; private set; }
    }
}
