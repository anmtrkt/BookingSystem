using BookingSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Post { get; set; } = string.Empty;
    public Role Role { get; set; }
}
