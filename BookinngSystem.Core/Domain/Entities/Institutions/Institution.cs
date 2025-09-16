using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Events;
using BookingSystem.Core.Domain.Events.RoomEvents;
using BookingSystem.Core.Domain.Models.InstitutionModels;
using BookingSystem.Core.Domain.ValueObjects;

namespace BookingSystem.Core.Domain.Entities.Institutions
{
    public class Institution : BaseEntity
    {
        public string Name { get; private set; }
        public string NormalizedName { get; private set; }
        public Guid? ParentId { get; private set; }
        public PriorityLevel PriorityLevel { get; private set; }


        // Navigation
        public Institution? Parent { get; private set; }
        private List<Branch> _branches = new();
        public ICollection<Branch> Branches => _branches;


        private  List<Institution> _children = new();
        public ICollection<Institution> Children => _children;


        private  List<User> _users = new();
        public ICollection<User> Employees =>_users;


#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        public Institution() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private Institution(string name, byte priority, Institution? parent)
        {
            Name = name;
            NormalizedName = name.ToUpper();
            PriorityLevel = PriorityLevel.Create(priority);
            if (parent != null)
            {
                UpdateParent(parent);
            }
        }

        public static Institution Create(string name,byte priority, Institution? parent = null)
        {
            var institution = new Institution(name, priority, parent);

            return institution;
        }
        public static InstitutionDto TransformToDto(Institution institution)
        {
            return new InstitutionDto()
            { Id = 
                institution.Id,
                Name = institution.Name, 
                PriorityLevel = institution.PriorityLevel, 
             /*   Parent = institution.Parent*//*!=null *//*
                ? Institution.TransformToDto(institution.Parent)
                : null */}; 
        }
        public void SetPriority(byte priority) {
            PriorityLevel = PriorityLevel.Create(priority);
        }
        public void AddBranch(Branch branch)
        {
            if (_branches.Contains(branch)) return;
            _branches.Add(branch);
            MarkAsModified();
        }
        public void RemoveBranch(Branch branch)
        {
            if (!_branches.Contains(branch)) return;
            _branches.Remove(branch);
            MarkAsModified();
        }
        public void AddChildren(Institution institution)
        {
            if (_children.Contains(institution)) return;
            _children.Add(institution);
            MarkAsModified();
        }
        public void RemoveChildren(Institution institution)
        {
            if (!_children.Contains(institution)) return;
            _children.Remove(institution);
            MarkAsModified();
        }
        public void AddUser(User user)
        {
            if (_users.Contains(user)) return;
            _users.Add(user);
            MarkAsModified();
        }
        public void RemoveUser(User user)
        {
            if (!_users.Contains(user)) return;
            _users.Remove(user);
            MarkAsModified();
        }

        /// <param name="newName"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name can't be null");

            Name = newName;
            NormalizedName = newName.ToUpper();
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
            DomainEvents.Raise<InstitutionHierarchyChangedEvent>(new InstitutionHierarchyChangedEvent(
            Id,
            parent?.Id,
            "Updated")); 

            MarkAsModified();
        }


    }
}
