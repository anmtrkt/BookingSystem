using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.Services.Models
{
    public class AuthenticationRequest
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;
        [Required]
        [Display(Name = "Password")]

        public string Password { get; set; } = null!;

    }
}
