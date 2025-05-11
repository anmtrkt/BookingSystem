using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Users.RoleSystem;
using BookingSystem.Core.Domain.ValueObjects;
using BookingSystem.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingSystem.Core.Domain.Entities
{
    public class User : BaseUser
    {
        public Guid InstitutionId { get; private set; }
        public Institution Institution { get; private set; }


        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();



        private User(
            string email,
            string phoneNumber,
            string surname,
            string name,
            Institution institution,
            string? patronymic = null)
            : base(email, phoneNumber, surname, name, patronymic)
        {
            InstitutionId = institution.Id;
            Institution = institution;
        }

        public static User Create(
            string email,
            string phoneNumber,
            string surname,
            string name,
            Institution institution,
            string? patronymic = null)
        {
            return new User(email, phoneNumber, surname, name, institution, patronymic);
        }

        public void UpdateEmail(string email)
        {
            base.UpdateEmail(email);
            MarkAsModified();
        }

        public void UpdatePhoneNumber(string phoneNumber)
        {
            base.UpdatePhoneNumber(phoneNumber);
            MarkAsModified();
        }

        public void UpdateInstitution(Institution institution)
        {
            InstitutionId = institution.Id;
            Institution = institution;
            MarkAsModified();
        }


        /// <param name="role"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRole(Role role, DateTime startDate, DateTime? endDate = null)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var existingRole = _userRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (existingRole != null)
            {
                existingRole.UpdateEndDate(endDate);
            }
            else
            {
                var userRole = UserRole.Create(Id, role.Id, startDate, endDate);
                _userRoles.Add(userRole);
            }

            MarkAsModified();
        }

        /// <param name="role"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole != null)
            {
                _userRoles.Remove(userRole);
                MarkAsModified();
            }
        }
    }
}