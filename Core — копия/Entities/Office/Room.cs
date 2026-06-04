using BookingSystem.Core.ValueObjects;

namespace BookingSystem.Core.Entities;

public class Room : BaseEntity
{
    public string Number { get; private set; }
    public Equipment Equipment { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public uint CountOfPlaces { get; private set; }
    public Office Office { get; private set; }
    public Guid OfficeId { get; private set; }
    public Schedule Schedule { get; private set; }
    public Guid ScheduleId { get; private set; }
#pragma warning disable CS8618
    private Room() { }
#pragma warning restore CS8618 
    public Room(
        string number,
        Guid officeId,
        Equipment equipment,
        uint countOfPlaces)
    {
        Schedule = new Schedule();
        ScheduleId = Schedule.Id;
        Number = number;
        OfficeId = officeId;
        Equipment = equipment;
        CountOfPlaces = countOfPlaces;
    }
    public void SetAvailable() => IsAvailable = true;
    public void SetUnavailable() => IsAvailable = false;
    public void ChangeNumber(string newNumber) => Number = newNumber;
    public void UpdateEquipment(Equipment newEquipment) => Equipment = newEquipment;
    public void UpdateCountOfPlaces(uint newCount) => CountOfPlaces = newCount;
}
