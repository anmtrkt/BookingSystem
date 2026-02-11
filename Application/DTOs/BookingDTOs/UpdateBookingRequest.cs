namespace BookingSystem.Application.DTOs;
public class UpdateBookingRequest
{
    public Guid BookingId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Guid> SubscribersId { get; set; } = new();
    public string Purpose { get; set; } = string.Empty;
}