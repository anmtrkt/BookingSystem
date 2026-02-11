using BookingSystem.Core.Entities;

namespace BookingSystem.Domain.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room?> GetByIdAsyncWithInclude(Guid id);
    Task<IEnumerable<Room>> GetByIdsAsync(IEnumerable<Guid> roomsId);
    Task<IEnumerable<Room>> GetByIdsAsyncWithInclude(IEnumerable<Guid> roomsId);

    Task<IEnumerable<Room>> GetAllAsync();
    Task<IEnumerable<Room>> GetAllAsyncWithInclude();

    Task AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(Guid id);
}