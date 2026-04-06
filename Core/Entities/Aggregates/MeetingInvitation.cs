namespace BookingSystem.Core.Entities.Aggregates;

public enum InvitationStatus
{
    Pending,
    Accepted,
    Declined
}

public class MeetingInvitation : BaseEntity
{
    public Guid MeetingId { get; private set; }
    public Meeting Meeting { get; private set; } = null!;

    public Guid InviteeId { get; private set; }
    public AppUser Invitee { get; private set; } = null!;

    public Guid InviterId { get; private set; }
    public AppUser Inviter { get; private set; } = null!;

    public InvitationStatus Status { get; private set; } = InvitationStatus.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }

#pragma warning disable CS8618
    private MeetingInvitation() { }
#pragma warning restore CS8618

    public MeetingInvitation(Guid meetingId, Guid inviteeId, Guid inviterId)
    {
        MeetingId = meetingId;
        InviteeId = inviteeId;
        InviterId = inviterId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Accept()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Можно принять только ожидающее приглашение.");
        
        Status = InvitationStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
    }

    public void Decline()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Можно отклонить только ожидающее приглашение.");
        
        Status = InvitationStatus.Declined;
        RespondedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Можно отменить только ожидающее приглашение.");
        
        Status = InvitationStatus.Declined;
        RespondedAt = DateTime.UtcNow;
    }
}
