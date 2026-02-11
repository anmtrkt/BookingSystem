using BookingSystem.Core.Entities.Aggregates;

namespace BookingSystem.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Meeting?> GetByIdAsync(Guid id);
    Task<Meeting?> GetByIdAsyncWithInclude(Guid id);

    Task<IEnumerable<Meeting>> GetAllAsync();
    Task<IEnumerable<Meeting>> GetAllAsyncWithInclude();

    Task<IEnumerable<Meeting>> GetByRoomIdAsync(Guid roomId, DateTime start, DateTime end);
    Task<IEnumerable<Meeting>> GetByRoomIdAsyncWithInclude(Guid roomId, DateTime start, DateTime end);

    Task<IEnumerable<Meeting>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Meeting>> GetByUserIdAsyncWithInclude(Guid userId);

    Task AddAsync(Meeting booking);
    Task UpdateAsync(Meeting booking);
    Task DeleteAsync(Guid id);
}