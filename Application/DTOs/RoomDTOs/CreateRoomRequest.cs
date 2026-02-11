namespace BookingSystem.Application.DTOs;
public class CreateRoomRequest
{
    public string Number { get; set; } = string.Empty;
    public Guid OfficeId { get; set; }
    public bool HasProjector { get; private set; }
    public bool HasSoundproofing { get; private set; }
    public bool HasWhiteboard { get; private set; }
    public bool HasInteractiveWhiteboard { get; private set; }
    public ushort NumberOfComputers { get; private set; }
    public bool HasVideoConferenceSystem { get; private set; }
    public bool HasMicrophones { get; private set; }
    public ushort NumberOfMicrophones { get; private set; }
    public bool HasAirConditioning { get; private set; }
    public bool HasTelevisions { get; private set; }
    public ushort NumberOfTelevisions { get; private set; }
    public bool HasWiFi { get; private set; }
    public uint CountOfPlaces { get; set; }


}
