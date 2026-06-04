using BookingSystem.Core.Entities;

namespace BookingSystem.Infrastructure.Repositories;


public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<AppUser>> GetByIdsAsync(IEnumerable<Guid> usersIds);

    Task<AppUser?> GetByEmailAsync(string email);
    Task<IEnumerable<AppUser>> GetAllAsync();
    Task AddAsync(AppUser user, string password);
    Task UpdateAsync(AppUser user);
    Task DeleteAsync(Guid id);
}