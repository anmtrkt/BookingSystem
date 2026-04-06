namespace BookingSystem.Application.DTOs;
public class CreateOfficeRequest
{
    public string Address { get; set; } = string.Empty;
    public Guid OrganizationId {  get; set; }
}
