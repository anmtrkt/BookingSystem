using BookingSystem.Core.Domain.Common;
using System.Collections.Generic;
using System.Linq;

namespace BookingSystem.Core.Domain.Entities.Users.RoleSystem
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> Permissions { get; private set; } = new List<string>();

        private Role() { } 
        private Role(string name, string description, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name can't be null or whitespace", nameof(name));

            Name = name;
            Description = description;
            Permissions = permissions ?? new List<string>();
        }

        public static Role Create(string name, string description, List<string> permissions = null)
        {
            return new Role(name, description, permissions);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name can't be null or whitespace", nameof(name));

            Name = name;
            MarkAsModified();
        }

        public void UpdateDescription(string description)
        {
            Description = description;
            MarkAsModified();
        }

        public void AddPermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("Permission can't be null or whitespace", nameof(permission));

            if (!Permissions.Contains(permission))
            {
                Permissions.Add(permission);
                MarkAsModified();
            }
        }

        public void RemovePermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("Permission can't be null or whitespace", nameof(permission));

            if (Permissions.Contains(permission))
            {
                Permissions.Remove(permission);
                MarkAsModified();
            }
        }
    }
}