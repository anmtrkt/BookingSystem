using BookingSystem.Core.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly BookingSystemDbContext _context;

    public RoomRepository(BookingSystemDbContext context)
    {
        _context = context;
    }
    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Room>().FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<Room?> GetByIdAsyncWithInclude(Guid id)
    {
        return await _context.Set<Room>().Include(r => r.Office).FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<IEnumerable<Room>> GetByIdsAsync(IEnumerable<Guid> roomsId)
    {
        return await _context.Rooms.Where(r => roomsId.Contains(r.Id))
                                                            .ToListAsync();

    }
    public async Task<IEnumerable<Room>> GetByIdsAsyncWithInclude(IEnumerable<Guid> roomsId)
    {
        return await _context.Rooms.Where(r => roomsId.Contains(r.Id))
                                                            .Include(r => r.Office)
                                                            .ToListAsync();

    }
    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Set<Room>().ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAllAsyncWithInclude()
    {
        return await _context.Set<Room>().Include(r => r.Office).ToListAsync();
    }

    public Task AddAsync(Room room)
    {
        _context.Set<Room>().Add(room);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Room room)
    {
        _context.Set<Room>().Update(room);
        return Task.CompletedTask;
    }
    public IQueryable<Room> AsQueryable()
    {
        return _context.Rooms.AsQueryable();
    }
    public async Task DeleteAsync(Guid id)
    {
        var room = await _context.Set<Room>().FindAsync(id);
        if (room != null)
        {
            _context.Set<Room>().Remove(room);

        }
    }
}