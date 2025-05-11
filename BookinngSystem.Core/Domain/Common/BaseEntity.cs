namespace BookingSystem.Core.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        protected BaseEntity() => Id = Guid.NewGuid();
        protected BaseEntity(Guid id) => Id = id;

        public void MarkAsModified()
        {
            ModifiedAt = DateTime.UtcNow;
            RowVersion = Guid.NewGuid().ToByteArray(); // update RowVersion for Optimistic lock
        }


    }
}
