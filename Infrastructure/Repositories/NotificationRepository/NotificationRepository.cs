using BookingSystem.Core.Entities;
using BookingSystem.Core.Entities.Aggregates;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingSystem.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{

    private readonly BookingSystemDbContext _context;

    public NotificationRepository(BookingSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications
                               .FirstOrDefaultAsync(n => n.Id == id);

    }
    public async Task<Notification?> GetByIdAsyncWithInclude(Guid id)
    {
        return await _context.Notifications
                               .Include(n => n.Receiver)
                               .FirstOrDefaultAsync(o => o.Id == id);

    }
    public async Task<IEnumerable<Notification>> GetByIdsAsync(IEnumerable<Guid> notificationsId)
    {
        return await _context.Notifications.Where(n => notificationsId.Contains(n.Id))
                                                            .ToListAsync();
    }
    public async Task<IEnumerable<Notification>> GetByIdsAsyncWithInclude(IEnumerable<Guid> notificationsId)
    {
        return await _context.Notifications.Include(n => n.Receiver).Where(n => notificationsId.Contains(n.Id))
                                                            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _context.Notifications
                               .ToListAsync();
    }
    public async Task<IEnumerable<Notification>> GetAllAsyncWithInclude()
    {
        return await _context.Notifications
                               .Include(n => n.Receiver).ToListAsync();
    }

    public Task AddAsync(Notification office)
    {
        _context.Set<Notification>().Add(office);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Notification office)
    {
        _context.Set<Notification>().Update(office);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var office = await _context.Set<Notification>().FindAsync(id);
        if (office != null)
        {
            _context.Set<Notification>().Remove(office);
        }
    }
}
