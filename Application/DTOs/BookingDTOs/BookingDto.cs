namespace BookingSystem.Application.DTOs;
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid CreatorId { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCancelled { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public List<string> SubscribersNames { get; set; } = new();
    public List<MeetingInvitationDto> Invitations { get; set; } = new();
}