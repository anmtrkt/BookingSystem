using BookingSystem.Core.Entities.Aggregates;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingSystemDbContext _context;

    public BookingRepository(BookingSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Meeting?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Meeting>().FirstOrDefaultAsync(b=>b.Id == id);
    }
    public async Task<Meeting?> GetByIdAsyncWithInclude(Guid id)
    {
        return await _context.Set<Meeting>()
            .Include(b => b.Creator)
            .Include(b => b.Room)
            .Include(b => b.Subscribers).FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Meeting>> GetAllAsync()
    {
        return await _context.Set<Meeting>()
            .ToListAsync();
    }
    public async Task<IEnumerable<Meeting>> GetAllAsyncWithInclude()
    {
        return await _context.Set<Meeting>()
            .Include(b => b.Creator)
            .Include(b => b.Room)
            .Include(b => b.Subscribers)
            .ToListAsync();
    }

    public async Task<IEnumerable<Meeting>> GetByRoomIdAsync(Guid roomId, DateTime start, DateTime end)
    {
        return await _context.Set<Meeting>()
            .Where(m => m.Room.Id == roomId &&
                        m.TimeRange.Start < end &&
                        m.TimeRange.End > start)
            .ToListAsync();
    }
    public async Task<IEnumerable<Meeting>> GetByRoomIdAsyncWithInclude(Guid roomId, DateTime start, DateTime end)
    {
        return await _context.Set<Meeting>()
            .Include(b => b.Creator)
            .Include(b => b.Room)
            .Include(b => b.Subscribers)
            .Where(m => m.Room.Id == roomId &&
                        m.TimeRange.Start < end &&
                        m.TimeRange.End > start)
            .ToListAsync();
    }

    public async Task<IEnumerable<Meeting>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Set<Meeting>()
            .Where(m => m.Creator.Id == userId)
            .ToListAsync();
    }
    public async Task<IEnumerable<Meeting>> GetByUserIdAsyncWithInclude(Guid userId)
    {
        return await _context.Set<Meeting>()
            .Include(b => b.Creator)
            .Include(b => b.Room)
            .Include(b => b.Subscribers)
            .Where(m => m.Creator.Id == userId)
            .ToListAsync();
    }

    public Task AddAsync(Meeting booking)
    {
        _context.Set<Meeting>().Add(booking);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Meeting booking)
    {
        _context.Set<Meeting>().Update(booking);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await _context.Set<Meeting>().FindAsync(id);
        if (booking != null)
        {
            _context.Set<Meeting>().Remove(booking);
            await Task.CompletedTask;
        }
    }
}