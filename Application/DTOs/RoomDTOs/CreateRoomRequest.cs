namespace BookingSystem.Application.DTOs;
public class CreateRoomRequest
{
    public string Number { get; set; } = string.Empty;
    public Guid OfficeId { get; set; }
    public bool HasProjector { get;  set; }
    public bool HasSoundproofing { get;  set; }
    public bool HasWhiteboard { get; set; }
    public bool HasInteractiveWhiteboard { get; set; }
    public ushort NumberOfComputers { get; set; }
    public bool HasVideoConferenceSystem { get; set; }
    public bool HasMicrophones { get; set; }
    public ushort NumberOfMicrophones { get; set; }
    public bool HasAirConditioning { get; set; }
    public bool HasTelevisions { get; set; }
    public ushort NumberOfTelevisions { get; set; }
    public bool HasWiFi { get;  set; }
    public uint CountOfPlaces { get; set; }


}
