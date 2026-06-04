using BookingSystem.Core.Entities;

namespace BookingSystem.Infrastructure.Repositories;


public interface IOfficeRepository
{
    Task<Office?> GetByIdAsync(Guid id);
    Task<Office?> GetByIdAsyncWithInclude(Guid id);

    Task<IEnumerable<Office>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<Office>> GetByIdsAsyncWithInclude(IEnumerable<Guid> ids);

    Task<IEnumerable<Office>> GetAllAsync();
    Task<IEnumerable<Office>> GetAllAsyncWithInclude();

    Task AddAsync(Office office);
    Task UpdateAsync(Office office);
    Task DeleteAsync(Guid id);
}