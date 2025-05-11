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

        private Institution(string name)
        {
            Name = name;
        }

        public static Institution Create(string name, Institution? parent = null)
        {
            var institution = new Institution(name)
            {
                Parent = parent,
                ParentId = parent?.Id
            };

            parent?._children.Add(institution);
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
        public void UpdateParent(Institution parent)
        {
            ParentId = parent.Id;
            Parent = parent;
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

    }
}
