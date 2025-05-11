// Core/Domain/Entities/Meeting.cs
using BookingSystem.Core.Domain.Common;

namespace BookingSystem.Core.Domain.Entities;

public class Meeting : BaseEntity
{
    public Guid RoomId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public Guid InstitutionId { get; private set; }
    public bool IsOverridden { get; private set; }
    public uint RequiredSeats { get; private set; }

    // Navigation
    public Institution Institution { get; private set; } = null!;
    public Room Room { get; private set; } = null!;
    public List<Official> Subscribers { get; private set; } = null!;

    private Meeting() { } 

    private Meeting(Guid roomId, DateTime startTime, DateTime endTime, Guid institutionId, List<Official> subscribers, uint quantity)
    {
        RoomId = roomId;
        StartTime = startTime;
        EndTime = endTime;
        InstitutionId = institutionId;
        Subscribers = subscribers;
        RequiredSeats = quantity;
        Validate();
    }


    // Fabric
    public static Meeting Create(
        Room room,
        DateTime startTime,
        DateTime endTime,
        uint quantity,
        Institution institution,
        List<Official> subscribers)
    { 
        return new Meeting(room.Id, startTime, endTime, institution.Id, subscribers, quantity) 
        {
            Room = room, 
            Institution = institution 
        };

        
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

    //Validation
    /// <exception cref="ArgumentException"></exception>
    private void Validate()
    {
        if (Room.CountOfPlaces < RequiredSeats)
            throw new ArgumentException("Not enough seats");
        if (StartTime >= EndTime)
            throw new ArgumentException("End should be later than start");

        if ((EndTime - StartTime).TotalHours > 24)
            throw new ArgumentException("Max duration can't be more than 24h");
    }
}