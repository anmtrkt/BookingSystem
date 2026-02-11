namespace BookingSystem.Application.DTOs;
public class UpdateOfficeRequest
{
    public Guid OfficeId { get; set; }
    public string Address { get; set; } = string.Empty;
    public List<Guid> RoomsId { get; set; } = new();
}
