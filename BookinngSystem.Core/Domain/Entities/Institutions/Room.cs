using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Events;
using BookingSystem.Core.Domain.Models.RoomModels;
using BookingSystem.Core.Domain.ValueObjects;
using System.Text.Json;

namespace BookingSystem.Core.Domain.Entities.Institutions
{
    public class Room : BaseEntity
    {
        public string Number { get; private set; }
        public Building Building { get; private set; }
        public Guid? BuildingId { get; private set; }
        public Equipment Equipment { get; private set; }
        public bool IsBooked { get; private set; } = false;
        public bool IsAvailable { get; private set; } = true;
        public uint CountOfPlaces { get; private set; }

        public Guid ScheduleId { get; private set; }
        public Schedule Schedule { get; private set; }
        private List<Meeting> _meetings = new();
        public ICollection<Meeting> Meetings => _meetings;


#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        public Room() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Room(
            string number,
            Building building,
            Guid buildingId,
            Equipment equipment,
            uint countOfPlaces)
        {
            this.Schedule = Schedule.Create(this);
            ScheduleId = Schedule.Id;
            Number = number;
            Building = building;
            BuildingId = buildingId;
            Equipment = equipment;
            CountOfPlaces = countOfPlaces;
            Validate();
        }
        public static Room Create(
            string number,
            Building building,
            Equipment equipment,
            uint countOfPlaces)
        {
            return new Room(number, building, building.Id, equipment, countOfPlaces );
        }
        public static RoomDto TransformToDto(Room room)
        {
            return new RoomDto { 
                Building = room.Building,
                Id = room.Id,
                Equipment = room.Equipment,
                CountOfPlaces = room.CountOfPlaces,
                IsAvailable = room.IsAvailable,
                IsBooked = room.IsBooked,
                Schedule = room.Schedule,
                Number = room.Number,

            };

        }
        public static List<RoomDto> TransformToDto(ICollection<Room> rooms)
        {
            List<RoomDto> dtos = new(rooms.Count);
            foreach(var room in rooms)
            {
                dtos.Add(TransformToDto(room));
            }
            return dtos;
        }
        public void SetAvailable() {
            IsAvailable = true;
            MarkAsModified();
        }
        public void SetUnavailable() {
            IsAvailable = false;
            MarkAsModified();
        }
        private bool Validate()
        {
            if (CountOfPlaces <= 0)
                return false;
            return true;

        }
        public void UpdateEquipment(Equipment newEquipment)
        {
            var oldEquipment = Equipment;
            Equipment = newEquipment;
            MarkAsModified();

            DomainEvents.Raise<RoomEquipmentUpdatedEvent>(new RoomEquipmentUpdatedEvent(
                Id,
                oldEquipment,
                newEquipment));
        }
        public void UpdateCountOfPlaces(uint newCount)
        {
            CountOfPlaces = newCount;
            MarkAsModified();
        }
        public void SetNumber(string number)
        {
            this.Number = number;
        }



    }
}