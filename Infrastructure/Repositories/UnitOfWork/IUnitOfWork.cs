namespace BookingSystem.Infrastructure.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
