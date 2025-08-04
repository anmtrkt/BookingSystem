using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Models.BuildingModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Entities.Institutions
{
    public class Building : BaseEntity
    {

        public string Address {  get; private set; }
        public Guid BranchId { get; private set; }
        public Branch Branch { get; private set; }
        public ICollection<Room> Rooms =>_rooms;

#pragma warning disable S2933 // Fields that are only assigned in the constructor should be "readonly"
        private List<Room> _rooms;
#pragma warning restore S2933 // Fields that are only assigned in the constructor should be "readonly"
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        public Building() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Building(string address, Branch branch, Guid branchId, List<Room> rooms) {
            Address = address;
            BranchId = branchId;
            Branch = branch;
            _rooms = rooms;
        }
        public static Building Create(string address, Branch branch, List<Room>? rooms = null)
        {
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
            return new Building(
                address,
                branch,
                branch.Id,
                rooms ??= new List<Room>());
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
        }
        public static BuildingDto TransformToDto(Building building)
        {
            return new BuildingDto()
            {
                Id = building.Id,
                Address = building.Address,
                Branch = Branch.TransformToDto(building.Branch),
                Rooms = Room.TransformToDto(building.Rooms),
            };
        }
        public static List<BuildingDto> TransformToDto(ICollection<Building> buildings)
        {
            List<BuildingDto> dtos = new(buildings.Count);
            foreach (var building in buildings)
            {
                dtos.Add(TransformToDto(building));
            }
            return dtos;
        }
        public void SetAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be null or whitespace.", nameof(address));

            Address = address;
        }


        public void SetBranch(Branch branch)
        {
            if (branch == null)
                throw new ArgumentNullException(nameof(branch));

            Branch = branch;
            BranchId = branch.Id;
        }

        public void AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _rooms.Add(room);
        }

        public void RemoveRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _rooms.Remove(room);
        }

    }
}
