using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services
{
    public interface IUserService
    {
        public Task<UserDto> GetUserByIdAsync(Guid id);
        public Task<UserDto> GetUserByEmailAsync(string email);
        /*        public Task<UserDto> GetUserByPhoneAsync(string phoneNumber);
                public Task<UserDto> GetUserByNameAsync(string name);*/
        public Task<UserDto> CreateUserAsync(CreateUserRequest request);
        public Task<UserDto> UpdateUserAsync(UpdateUserRequest request);
        public Task DeleteUserAsync(Guid id);
        /*        public Task<List<UserDto>?> GetUsersByIdsAsync(ICollection<Guid> ids);*/
        public Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}
