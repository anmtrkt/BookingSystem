using BookingSystem.Core.Domain.Common;

namespace BookingSystem.Core.Domain.Entities
{
    public class Institution : BaseEntity
    {
        public string Name { get; private set; }
        public Guid? ParentId { get; private set; }


        // Navigation
        public Institution? Parent { get; private set; }
        private readonly List<Institution> _children = new();
        public IReadOnlyCollection<Institution> Children => _children.AsReadOnly();

        private readonly List<Room> _rooms = new();
        public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

        private Institution(string name)
        {
            Name = name;
        }

        public static Institution Create(string name, Institution? parent = null)
        {
            var institution = new Institution(name);
            if (parent != null)
            {
                institution.UpdateParent(parent);
            }
            return institution;
        }

        /// <param name="newName"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name can't be null");

            Name = newName;
            MarkAsModified();
        }
        private bool HasCycle(Institution parent)
        {
            var current = this;
            while (current != null)
            {
                if (current == parent) return true;
                current = current.Parent;
            }
            return false;
        }

        public void UpdateParent(Institution parent)
        {
            if (HasCycle(parent)) throw new InvalidOperationException("Cycle detected in institution hierarchy");
            ParentId = parent.Id;
            Parent = parent;
            parent._children.Add(this);
            MarkAsModified();
        }
        public void AddRoom(Room room)
        {
            if (_rooms.Contains(room)) return;
            _rooms.Add(room);
            room.OwnerInstitution = this;
            MarkAsModified();
        }

        public void RemoveRoom(Room room)
        {
            if (!_rooms.Contains(room)) return;
            _rooms.Remove(room);
            room.OwnerInstitution = null;
            MarkAsModified();
        }

    }
}
