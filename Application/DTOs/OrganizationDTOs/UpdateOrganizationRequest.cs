namespace BookingSystem.Application.DTOs;
public class UpdateOrganizationRequest
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Guid>? OfficesId { get; set; }
}
