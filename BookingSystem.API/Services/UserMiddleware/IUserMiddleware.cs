using BookingSystem.API.Services.Models;
using BookingSystem.Core.Domain.Entities.Users;

namespace BookingSystem.API.Services.UserServices
{
    public interface IUserMiddleware
    {
        public Task<object?> Registration(RegisterRequest request);
        public Task<AuthResponse> Authenticate(User user);
    }
}
