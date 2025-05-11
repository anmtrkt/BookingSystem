

// Ignore Spelling: Json

using System.Diagnostics;
using System.Text.Json;

namespace BookingSystem.Core.Domain.ValueObjects
{
    public class Equipment : ValueObject
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
        public JsonDocument ToJson()
        {
            return JsonSerializer.SerializeToDocument(this);
        }


        public static Equipment FromJson(string json)
        {
           
            return JsonSerializer.Deserialize<Equipment>(json);

        }
        public static Equipment FromJson(JsonDocument json)
        {
            return JsonSerializer.Deserialize<Equipment>(json);
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return HasProjector;
            yield return HasSoundproofing;
            yield return HasWhiteboard;
            yield return HasInteractiveWhiteboard;
            yield return NumberOfComputers;
            yield return HasVideoConferenceSystem;
            yield return HasMicrophones;
            yield return NumberOfMicrophones;
            yield return HasAirConditioning;
            yield return HasTelevisions;
            yield return NumberOfTelevisions;
            yield return HasWiFi;
        }
    }
}