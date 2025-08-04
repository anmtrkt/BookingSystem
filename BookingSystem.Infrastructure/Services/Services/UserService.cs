using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Models.UserModels;
using BookingSystem.Infrastructure.Persistence;
using BookingSystem.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Services
{
    public class UserService : IUserService
    {
        private readonly BookingSystemDbContext _dbContext;
        private readonly UserManager<User> _userManager;


        public UserService(BookingSystemDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;

        }

        public async Task<IEnumerable<UserDto>> GetManagedUsersAsync(User user)
        {
            var users =  await _dbContext.Users.Where(u => user.ManagerUsersId.Contains(u.Id)).ToListAsync();
            List<UserDto> usersDto = new (users.Count);
            users.ForEach(u => usersDto.Add(User.TransformToDto(u)));
            return usersDto;
        }


        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException($"User with ID: {userId} not found.");

            return User.TransformToDto(user);
        }


        public async Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task DeleteAsync(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException($"User with ID: {userId} not found.");

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<UserDto>?> GetUsersByIdsAsync(ICollection<Guid> usersIds)
        {
           
             var users = await _dbContext.Users
                .Where(u => usersIds.Contains(u.Id)).ToListAsync();
            List<UserDto> usersDto = new(users.Count);
            users.ForEach(u => usersDto.Add(User.TransformToDto(u)));
            return usersDto;

        }
        
        public async Task<List<UserDto>> GetUsersByInstitutionAsync(Guid institutionId)
        {
            var users =  await _dbContext.Users.Where(u => u.InstitutionId == institutionId).ToListAsync();
            List<UserDto> usersDto = new(users.Count);
            users.ForEach(u => usersDto.Add(User.TransformToDto(u)));
            return usersDto;
        }
        /*public async Task<User> GetUserByEmail(string email)
        {

        }*/
    }
}
