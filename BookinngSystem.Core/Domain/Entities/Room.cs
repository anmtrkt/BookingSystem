/*using BookingSystem.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Entities
{
    public class Room : BaseEntity
    {
        public Guid BuildingId { get; private set; }
        public JsonDocument Equipment { get; private set; }
        public bool isBooked { get; private set; } = false;
        public uint CountOfPlaces { get; private set; }
        public Institution OwnerInstitution { get; private set; }

        private Room(Guid buildingId, JsonDocument equipment, uint countOfPlaces, Institution ownerInstitution)
        {
            BuildingId = buildingId;
            Equipment = equipment;
            CountOfPlaces = countOfPlaces;
            OwnerInstitution = ownerInstitution;
            Validate();
        }

        public static Room Create(Guid buildingId, JsonDocument equipment, uint countOfPlaces, Institution ownerInstitution)
        {
            return new Room(buildingId, equipment, countOfPlaces, ownerInstitution);
        }


        /// <exception cref="ArgumentException"></exception>
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Equipment.ToString()))
                throw new ArgumentException("Equipment can't be null");
        }
    }
}*/
using BookingSystem.Core.Domain.Common;

namespace BookingSystem.Core.Domain.Entities
{
    public class Room : BaseEntity
    {
        public BuildingId BuildingId { get; }
        public RoomEquipment Equipment { get; }
        public Capacity Capacity { get; }
        public InstitutionId OwnerInstitutionId { get; }

        // Навигационное свойство (только для EF Core)
        public Institution? OwnerInstitution { get; private set; }

        private Room(
            BuildingId buildingId,
            RoomEquipment equipment,
            Capacity capacity,
            InstitutionId ownerInstitutionId)
        {
            BuildingId = buildingId;
            Equipment = equipment;
            Capacity = capacity;
            OwnerInstitutionId = ownerInstitutionId;
        }

        public static Result<Room> Create(
            BuildingId buildingId,
            RoomEquipment equipment,
            Capacity capacity,
            InstitutionId ownerInstitutionId)
        {
            if (capacity.Value == 0)
                return Result.Failure<Room>("Capacity must be positive");

            return new Room(buildingId, equipment, capacity, ownerInstitutionId);
        }
    }

    // Value Objects
    public record BuildingId(Guid Value);
    public record InstitutionId(Guid Value);

    public sealed class RoomEquipment
    {
        private readonly HashSet<string> _items;

        public IReadOnlyCollection<string> Items => _items.ToList().AsReadOnly();

        public RoomEquipment(IEnumerable<string> equipment)
        {
            _items = equipment.ToHashSet();
            Validate();
        }

        private void Validate()
        {
            if (_items.Count == 0)
                throw new ArgumentException("At least one equipment item required");
        }

        public bool HasEquipment(string item) => _items.Contains(item);
    }

    public record Capacity(uint Value)
    {
        public static implicit operator uint(Capacity c) => c.Value;
    }
}