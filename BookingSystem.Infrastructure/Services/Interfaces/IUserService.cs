using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Interfaces
{
    public interface IUserService
    {
      //public Task<User?> GetUserByEmailAsync(string email);
        public Task<IEnumerable<UserDto>> GetManagedUsersAsync(User user);
        public Task<UserDto> GetUserAsync(Guid userId);
        public Task UpdateAsync(User user);
        public Task DeleteAsync(Guid userId);
        public Task DeleteAsync(User userId);
        public Task<List<UserDto>?> GetUsersByIdsAsync(ICollection<Guid> ids);
        public Task<List<UserDto>> GetUsersByInstitutionAsync(Guid institutionId);
        
    }
}
