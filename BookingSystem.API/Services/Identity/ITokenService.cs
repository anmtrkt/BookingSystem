using BookingSystem.Core.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.API.Services.Identity
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> role);
    }
}
