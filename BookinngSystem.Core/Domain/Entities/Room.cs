using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.ValueObjects;
using System.Text.Json;

namespace BookingSystem.Core.Domain.Entities
{
    public class Room : BaseEntity
    {
        public Guid BuildingId { get; private set; }
        public JsonDocument Equipment { get; private set; }
        public bool IsBooked { get; private set; } = false;
        public uint CountOfPlaces { get; private set; }
        public Guid OwnerInstitutionId { get; private set; }
        public Institution OwnerInstitution { get; private set; }

        private Room(
            Guid buildingId,
            Equipment equipment,
            uint countOfPlaces,
            Institution ownerInstitution)
        {
            BuildingId = buildingId;
            Equipment = equipment.ToJson();
            CountOfPlaces = countOfPlaces;
            OwnerInstitution = ownerInstitution;
            OwnerInstitutionId = ownerInstitution.Id;
            Validate();
        }

        public static Room Create(
            Guid buildingId,
            Equipment equipment,
            uint countOfPlaces,
            Institution ownerInstitution)
        {
            return new Room(buildingId, equipment, countOfPlaces, ownerInstitution);
        }


        /// <exception cref="ArgumentException"></exception>
        private void Validate()
        {
            if (CountOfPlaces <= 0)
                throw new ArgumentException("Count of places must be greater than zero");
            if (Equipment == null || !Equipment.RootElement.ValueKind.Equals(JsonValueKind.Object))
                throw new ArgumentException("Equipment can't be null or empty");
        
        }
        public void UpdateEquipment(Equipment newEquipment)
        {
            Equipment = newEquipment.ToJson();
            MarkAsModified();
        }
        public void UpdateCountOfPlaces(uint newCount)
        {
            CountOfPlaces = newCount;
            MarkAsModified();
        }
    }
}