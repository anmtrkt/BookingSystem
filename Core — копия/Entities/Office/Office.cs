namespace BookingSystem.Core.Entities;

public class Office : BaseEntity
{
    public string Address { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; }
    public ICollection<Room> Rooms { get; private set; } = new List<Room>();


#pragma warning disable CS8618
    public Office() { }
#pragma warning restore CS8618 
    public Office(string address, Guid organizationId)
    {
        Address = address;
        OrganizationId = organizationId;
    }
    public void AddRoom(Room room)
    {
        if (!Rooms.Any(r => r.Id == room.Id))
        { 
            Rooms.Add(room); 
        }

        return;
    }

    public void ChangeAddress(string newAddress) => Address = newAddress;
    public void RemoveRoom(Room room)
    {
        var roomToRemove = Rooms.FirstOrDefault(r => r.Id == room.Id);
        if (roomToRemove != null)
        {
            Rooms.Remove(roomToRemove);
        }
    }
}

