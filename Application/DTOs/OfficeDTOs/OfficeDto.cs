namespace BookingSystem.Application.DTOs;
public class OfficeDto
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public IList<Guid> Rooms { get; set; } = new List<Guid>();
}