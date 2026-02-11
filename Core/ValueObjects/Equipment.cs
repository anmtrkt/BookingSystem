using System.Text.Json.Serialization;
namespace BookingSystem.Core.ValueObjects;
public class Equipment
{
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



    [JsonConstructor]
    private Equipment(
   bool hasProjector,
   bool hasSoundproofing,
   bool hasWhiteboard,
   bool hasInteractiveWhiteboard,
   ushort numberOfComputers,
   bool hasVideoConferenceSystem,
   bool hasMicrophones,
   ushort numberOfMicrophones,
   bool hasAirConditioning,
   bool hasTelevisions,
   ushort numberOfTelevisions,
   bool hasWiFi
)
    {
        HasProjector = hasProjector;
        HasSoundproofing = hasSoundproofing;
        HasWhiteboard = hasWhiteboard;
        HasInteractiveWhiteboard = hasInteractiveWhiteboard;
        NumberOfComputers = numberOfComputers;
        HasVideoConferenceSystem = hasVideoConferenceSystem;
        HasMicrophones = hasMicrophones;
        NumberOfMicrophones = numberOfMicrophones;
        HasAirConditioning = hasAirConditioning;
        HasTelevisions = hasTelevisions;
        NumberOfTelevisions = numberOfTelevisions;
        HasWiFi = hasWiFi;
    }
    private Equipment() { }
    private Equipment(
        bool hasProjector,
        bool hasSoundproofing,
        bool hasWhiteboard,
        bool hasInteractiveWhiteboard,
        bool hasVideoConferenceSystem,
        bool hasMicrophones,
        bool hasWiFi,
        bool hasAirConditioning,
        bool hasTelevions,
        ushort numberOfMicrophones,
        ushort numberOfTelevisions,
        ushort numberOfComputers
        )
    {
        HasProjector = hasProjector;
        HasSoundproofing = hasSoundproofing;
        HasWhiteboard = hasWhiteboard;
        HasInteractiveWhiteboard = hasInteractiveWhiteboard;
        NumberOfComputers = numberOfComputers;
        HasVideoConferenceSystem = hasVideoConferenceSystem;
        HasMicrophones = hasMicrophones;
        NumberOfMicrophones = numberOfMicrophones;
        HasAirConditioning = hasAirConditioning;
        HasTelevisions = hasTelevions;
        NumberOfTelevisions = numberOfTelevisions;
        HasWiFi = hasWiFi;
    }

    public static Equipment Create(
        bool hasProjector,
        bool hasSoundproofing,
        bool hasWhiteboard,
        bool hasInteractiveWhiteboard,
        bool hasVideoConferenceSystem,
        bool hasMicrophones,
        bool hasWiFi,
        bool hasAirConditioning,
        bool hasTelevions,
        ushort numberOfMicrophones = 0,
        ushort numberOfTelevisions = 0,
        ushort numberOfComputers = 0)
    {
        return new Equipment(
            hasProjector,
            hasSoundproofing,
            hasWhiteboard,
            hasInteractiveWhiteboard,
            hasVideoConferenceSystem,
            hasMicrophones,
            hasWiFi,
            hasAirConditioning,
            hasTelevions,
            numberOfMicrophones,
            numberOfTelevisions,
            numberOfComputers);
    }
}
