namespace BookingSystem.Core.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; private set; }

        public void MarkAsModified() => ModifiedAt = DateTime.UtcNow;

        protected BaseEntity() => Id = Guid.NewGuid(); 
        protected BaseEntity(Guid id) => Id = id;
    }
}
