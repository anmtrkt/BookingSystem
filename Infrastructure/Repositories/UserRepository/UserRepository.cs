using BookingSystem.Core.Entities;
using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Identity;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BookingSystemDbContext _context;

    public UserRepository(BookingSystemDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var appUser = await _context.Users.FindAsync(id);
        return appUser != null ? MapToDomain(appUser) : null;
    }
    public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> usersIds)
    {
        var appUsers = await _context.Users.Where(u => usersIds.Contains(u.Id))
                                                            .ToListAsync();
        return appUsers.Select(MapToDomain);
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return appUser != null ? MapToDomain(appUser) : null;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var appUsers = await _context.Users.ToListAsync();
        return appUsers.Select(MapToDomain);
    }

    public async Task AddAsync(User user)
    {
        var appUser = new AppUser
        {
            UserName = user.Email,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Post = user.Post
        };

        await _context.Users.AddAsync(appUser);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(User user)
    {
        var appUser = await _context.Users.FindAsync(user.Id);
        if (appUser != null)
        {
            appUser.Email = user.Email;
            appUser.UserName = user.Email;
            appUser.Name = user.Name;
            appUser.Surname = user.Surname;
            appUser.Post = user.Post;

            _context.Users.Update(appUser);
            await Task.CompletedTask;
        }
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

    private static User MapToDomain(AppUser appUser)
    {

        return new User(appUser.Email, appUser.Post, appUser.Surname, appUser.Name, null, appUser.PhoneNumber)
        {
            Id = appUser.Id 
        };
    }
}