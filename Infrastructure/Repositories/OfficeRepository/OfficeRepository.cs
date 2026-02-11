using BookingSystem.Core.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class OfficeRepository : IOfficeRepository
{
    private readonly BookingSystemDbContext _context;

    public OfficeRepository(BookingSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Office?> GetByIdAsync(Guid id)
    {
        return await _context.Offices
                               .FirstOrDefaultAsync(o => o.Id == id);
       
    }
    public async Task<Office?> GetByIdAsyncWithInclude(Guid id)
    {
        return await _context.Offices
                               .Include(o => o.Rooms)
                               .FirstOrDefaultAsync(o => o.Id == id);

    }
    public async Task<IEnumerable<Office>> GetByIdsAsync(IEnumerable<Guid> officiesId)
    {
        return await _context.Offices.Where(o => officiesId.Contains(o.Id))
                                                            .ToListAsync();
    }
    public async Task<IEnumerable<Office>> GetByIdsAsyncWithInclude(IEnumerable<Guid> officiesId)
    {
        return await _context.Offices.Include(o => o.Rooms).Where(o => officiesId.Contains(o.Id))
                                                            .ToListAsync();
    }

    public async Task<IEnumerable<Office>> GetAllAsync()
    {
        return await _context.Offices
                               .ToListAsync();
    }
    public async Task<IEnumerable<Office>> GetAllAsyncWithInclude()
    {
        return await _context.Offices
                               .Include(o => o.Rooms).ToListAsync();
    }

    public Task AddAsync(Office office)
    {
        _context.Set<Office>().Add(office);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Office office)
    {
        _context.Set<Office>().Update(office);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var office = await _context.Set<Office>().FindAsync(id);
        if (office != null)
        {
            _context.Set<Office>().Remove(office);
        }
    }
}