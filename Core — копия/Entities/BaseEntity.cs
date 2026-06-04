namespace BookingSystem.Core.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; init; }
    public bool IsArchive { get; private set; } = false;
    protected BaseEntity() => Id = Guid.NewGuid();
    public void SetArchive() => IsArchive = true;
    public void SetUnarchive() => IsArchive = false;
}

