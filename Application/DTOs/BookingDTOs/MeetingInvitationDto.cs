namespace BookingSystem.Application.DTOs;

public class MeetingInvitationDto
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public Guid InviteeId { get; set; }
    public string InviteeName { get; set; } = string.Empty;
    public Guid InviterId { get; set; }
    public string InviterName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public class CreateInvitationRequest
{
    public Guid MeetingId { get; set; }
    public List<Guid> InviteesIds { get; set; } = new();
}

public class RespondToInvitationRequest
{
    public Guid InvitationId { get; set; }
    public bool Accept { get; set; } // true = принять, false = отклонить
}
