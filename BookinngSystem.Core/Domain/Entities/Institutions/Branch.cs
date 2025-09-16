using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Events;
using BookingSystem.Core.Domain.Events.RoomEvents;
using BookingSystem.Core.Domain.Models.BranchModels;

namespace BookingSystem.Core.Domain.Entities.Institutions
{
    public class Branch : BaseEntity
    {
        public Institution Institution { get; private set; }
        public Guid InstitutionId { get; private set; }
#pragma warning disable S2933 // Fields that are only assigned in the constructor should be "readonly"
        private List<Building> _buildings = new();
#pragma warning restore S2933 // Fields that are only assigned in the constructor should be "readonly"
        public ICollection<Building> Buildings => _buildings;
        public string Name { get; private set; }

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Branch() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private Branch(
            Institution institution,
            string name,
            string address)
        {
            Institution = institution;
            InstitutionId = institution.Id;
            Name = name;
            Address = address;
        }

        public static Branch Create(
            Institution institution,
            string name,
            string address)
        {
            return new Branch(institution, name, address);
        }
        public static BranchDto TransformToDto(Branch branch)
        {
            return new BranchDto()
            {
                Id = branch.Id,
                Institution = Institution.TransformToDto(branch.Institution),
                Buildings = Building.TransformToDto(branch.Buildings),
                Name = branch.Name,
                Address = branch.Address
            };
        }
        public void UpdateAddress(string newAddress)
        {
            Address = newAddress;
            MarkAsModified();
        }
        public void AddBuilding(Building building)
        {
            if (_buildings.Contains(building)) return;
            _buildings.Add(building);
            MarkAsModified();
        }
        public void RemoveBuilding(Building building)
        {
            if (!_buildings.Contains(building)) return;
            _buildings.Remove(building);
            MarkAsModified();
        }
    }
}