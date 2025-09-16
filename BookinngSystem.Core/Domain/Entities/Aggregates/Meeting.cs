// Core/Domain/Entities/Meeting.cs
using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Events;
using BookingSystem.Core.Domain.Events.BookingEvents;
using BookingSystem.Core.Domain.Models.MeetingsModels;
using BookingSystem.Core.Domain.ValueObjects;

namespace BookingSystem.Core.Domain.Entities.Aggregates;

public class Meeting : BaseEntity
{
    public Guid CreatorId { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid InstitutionId { get; private set; }
    public bool IsOverridden { get; private set; }
    public TimeRange TimeRange { get; private set; }
    public bool IsCancelled { get; private set; } = false;

    // Navigation
    public Institution Institution { get; private set; }
    public User Creator { get; private set; }
    public Room Room { get; private set; }
    public ICollection<User> Subscribers { get; private set; }

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
    private Meeting() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

    private Meeting(
        User creator,
        List<User> subscribers,
        Room room,
        Institution institution, DateTime start, DateTime end
        )
    {
        
        CreatorId = creator.Id;
        Creator = creator;

        Subscribers = subscribers;
        Subscribers.Add(creator);

        RoomId = room.Id;
        Room = room;


        InstitutionId = institution.Id;
        Institution = institution;
        
       

        TimeRange = TimeRange.Create(start, end, this);


        DomainEvents.Raise<MeetingCreatedEvent>(new MeetingCreatedEvent(
    Id,
    CreatorId,
    RoomId,
    TimeRange.Start,
    TimeRange.End
    ));

    }

    public static Meeting Create(
        User creator,
        Room room,
        DateTime startTime,
        DateTime endTime,
        Institution institution,
        List<User>? subscribers = null)
    {
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
        return new Meeting(
            creator,
            subscribers??=new(),
            room,
            institution,
            startTime,endTime);
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
    }
    public static List<MeetingDto> TransformToDto(List<Meeting> meetings)
    {
        List<MeetingDto> result = new(meetings.Count);
        foreach (var meeting in meetings)
        {
            result.Add(TransformToDto(meeting));
        }
        return result;
    }
    public static MeetingDto TransformToDto(Meeting meeting)
    {
        if (meeting.Institution == null) throw new ArgumentNullException();
        return new MeetingDto()
        {
            Id = meeting.Id,
            Creator = User.TransformToDto(meeting.Creator),
            TimeRange = meeting.TimeRange,
            IsCancelled = meeting.IsCancelled,
            Institution = Institution.TransformToDto(meeting.Institution),
            Room = Room.TransformToDto(meeting.Room),
            Subscribers = User.TransformToDto(meeting.Subscribers)

        };
    }
    public void ChangeTime(DateTime newStart, DateTime newEnd)
    {
        TimeRange = TimeRange.Create(newStart, newEnd, this);
        DomainEvents.Raise<MeetingTimeChangedEvent>(new MeetingTimeChangedEvent());
    }
    public void ChangeSubscribers(List<User> newSubs)
    {
        Subscribers = newSubs;
        MarkAsModified();
    }
    public void Cancel(string reason)
    {
        IsOverridden = true;
        IsCancelled = true;
        DomainEvents.Raise<MeetingCanceledEvent>(new MeetingCanceledEvent(
            Id,
            CreatorId,
            reason));
        MarkAsModified();
    }
    public void Uncancel()
    {
        IsCancelled = false;

        DomainEvents.Raise<MeetingUncanceledEvent>(new MeetingUncanceledEvent(
            Id,
            CreatorId
            ));
        MarkAsModified();
    }
    public void Subscribe(User user)
    {
        Subscribers.Add(user);
        MarkAsModified();
    }
    public void Unsubscribe(User user)
    {
        Subscribers.Remove(user);
        MarkAsModified();
    }

    //Changing
    /// <exception cref="InvalidOperationException"></exception>
    public void MarkAsOverridden()
    {
        if (IsOverridden)
            throw new InvalidOperationException("Booking are already taken");

        IsOverridden = true;
        MarkAsModified();
    }
}