namespace BookingSystem.Application.DTOs;
public class UpdateUserRequest
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Post { get; set; }
    public string? PhoneNumber { get; set; }
}
