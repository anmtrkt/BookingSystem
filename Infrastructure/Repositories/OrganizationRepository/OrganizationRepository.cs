using BookingSystem.Core.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly BookingSystemDbContext _context;

    public OrganizationRepository(BookingSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(Guid id)
    {
        return await _context.Organizations
                       .FirstOrDefaultAsync(o => o.Id == id);
    }
    public async Task<Organization?> GetByIdAsyncWithInclude(Guid id)
    {
        return await _context.Organizations
                       .Include(o => o.Officies)
                       .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Organization>> GetAllAsync()
    {
        return await _context.Organizations
                       .ToListAsync();
    }
    public async Task<IEnumerable<Organization>> GetAllAsyncWithInclude()
    {
        return await _context.Organizations
                       .Include(o => o.Officies).ToListAsync();
    }

    public Task AddAsync(Organization organization)
    {
        _context.Set<Organization>().Add(organization);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Organization organization)
    {
        _context.Set<Organization>().Update(organization);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var organization = await _context.Set<Organization>().FindAsync(id);
        if (organization != null)
        {
            _context.Set<Organization>().Remove(organization);
        }
    }
}