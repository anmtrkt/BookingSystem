using BookingSystem.Core.Entities;
using BookingSystem.Core.Entities.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingSystem.Infrastructure.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id);
    Task<Notification?> GetByIdAsyncWithInclude(Guid id);

    Task<IEnumerable<Notification>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<Notification>> GetByIdsAsyncWithInclude(IEnumerable<Guid> ids);

    Task<IEnumerable<Notification>> GetAllAsync();
    Task<IEnumerable<Notification>> GetAllAsyncWithInclude();

    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task DeleteAsync(Guid id);
}
