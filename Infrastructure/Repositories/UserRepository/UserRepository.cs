using BookingSystem.Core.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BookingSystemDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(BookingSystemDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<AppUser?> GetByIdAsync(Guid id)
    {
        var appUser = await _context.Users.FindAsync(id);
        return appUser != null ? appUser : null;
    }
    public async Task<IEnumerable<AppUser>> GetByIdsAsync(IEnumerable<Guid> usersIds)
    {
        var appUsers = await _context.Users.Where(u => usersIds.Contains(u.Id))
                                                            .ToListAsync();
        return appUsers;
    }
    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        return appUser != null ? appUser : null;
    }

    public async Task<IEnumerable<AppUser>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(AppUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) throw new ApplicationException(string.Join(", ", result.Errors.Select(x => x?.ToString() ?? "")));
        
    }

    public async Task UpdateAsync(AppUser user)
    {

        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var appUser = await _context.Users.FindAsync(id);
        if (appUser != null)
        {
            _context.Users.Remove(appUser);
            await Task.CompletedTask;
        }
    }
}