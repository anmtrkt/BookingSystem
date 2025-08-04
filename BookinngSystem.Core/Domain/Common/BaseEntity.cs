using BookingSystem.Core.Domain.Events;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Core.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ModifiedAt { get; private set; }
        public bool IsArchive { get; private set; } = false;



        protected BaseEntity() {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsModified()
        {
            ModifiedAt = DateTime.UtcNow;
            
        }
        public void SetArchive()
        {
            IsArchive = true;
            MarkAsModified();
        }
        public void SetUnarchive()
        {
            IsArchive = false;
            MarkAsModified();
        }



    }
}
