using BookingSystem.Core.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id);
    Task<Organization?> GetByIdAsyncWithInclude(Guid id);

    Task<IEnumerable<Organization>> GetAllAsync();
    Task<IEnumerable<Organization>> GetAllAsyncWithInclude();

    Task AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Guid id);
}