using BookingSystem.Core.ValueObjects;

namespace BookingSystem.Core.Entities.Aggregates;

public class Meeting : BaseEntity
{
    public AppUser Creator { get; private set; }
    public Guid CreatorId { get; private set; }
    public Room Room { get; private set; }
    public Guid RoomId { get; private set; }

    public TimeRange TimeRange { get; private set; }
    public string Reason { get; private set; }
    public bool IsCancelled { get; private set; } = false;
    public ICollection<AppUser> Subscribers { get; private set; } = new List<AppUser>();
    public ICollection<MeetingInvitation> Invitations { get; private set; } = new List<MeetingInvitation>();
#pragma warning disable CS8618 
    private Meeting() { }
#pragma warning restore CS8618 
    public Meeting(
        Guid creatorId,
        Guid roomId,
        string reason,
        DateTime startTime,
        DateTime endTime)
    {
        CreatorId = creatorId;
        RoomId = roomId;
        Reason = reason;
        TimeRange = new TimeRange(startTime, endTime);
    }

    public void UpdateTimeRange(DateTime newStartTime, DateTime newEndTime) => TimeRange = new TimeRange(newStartTime, newEndTime);
    public void Uncancel() => IsCancelled = false;
    public void Cancel() => IsCancelled = true;
    public void Subscribe(AppUser user)
    {
        if (!Subscribers.Any(s => s.Id == user.Id))
        {
            Subscribers.Add(user);
        }
    }
    public void Unsubscribe(AppUser user)
    {
        var subscriberToRemove = Subscribers.FirstOrDefault(s => s.Id == user.Id);
        if (subscriberToRemove != null)
        {
            Subscribers.Remove(subscriberToRemove);
        }
    }

    public MeetingInvitation CreateInvitation(Guid inviteeId, Guid inviterId)
    {
        // Проверяем, нет ли уже активного приглашения для этого пользователя
        var existingInvitation = Invitations
            .FirstOrDefault(i => i.InviteeId == inviteeId && i.Status == InvitationStatus.Pending);
        
        if (existingInvitation != null)
        {
            throw new InvalidOperationException("Приглашение для этого пользователя уже существует.");
        }

        var invitation = new MeetingInvitation(Id, inviteeId, inviterId);
        Invitations.Add(invitation);
        return invitation;
    }
}