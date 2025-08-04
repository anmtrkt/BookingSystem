namespace BookingSystem.API.Services.Models
{
    public class AuthenticationRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
