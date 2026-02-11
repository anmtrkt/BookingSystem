namespace BookingSystem.Application.DTOs;
public class CreateBookingRequest
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public List<Guid> SubscribersId { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
}