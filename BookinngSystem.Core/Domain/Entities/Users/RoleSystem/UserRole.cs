using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Users.RoleSystem;
using BookingSystem.Core.Domain.Entities.Users;
using System;

namespace BookingSystem.Core.Domain.Entities.Users.RoleSystem
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }


        public User User { get; private set; }
        public Role Role { get; private set; }

        private UserRole() { } 

        private UserRole(Guid userId, Guid roleId, DateTime startDate, DateTime? endDate)
        {
            UserId = userId;
            RoleId = roleId;
            StartDate = startDate;
            EndDate = endDate;
        }

        public static UserRole Create(Guid userId, Guid roleId, DateTime startDate, DateTime? endDate = null)
        {
            return new UserRole(userId, roleId, startDate, endDate);
        }

        public void UpdateEndDate(DateTime? endDate)
        {
            EndDate = endDate;
            MarkAsModified();
        }
    }
}