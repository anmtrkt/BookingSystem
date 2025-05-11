using BookingSystem.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Common
{
    public class BaseUser : BaseEntity
    {
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Surname { get; private set; }
        public string Name { get; private set; }
        public string? Patronymic { get; private set; }
        public string FullName { get; private set; }
        public string NormalizedEmail { get; private set; }
        public string NormalizedPhoneNumber { get; private set; }
        public string NormalizedSurname { get; private set; }
        public string NormalizedName { get; private set; }
        public string? NormalizedPatronymic { get; private set; }
        public string NormalizedFullName { get; private set; }

        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="surname"></param>
        /// <param name="name"></param>
        /// <param name="patronymic"></param>
        /// <exception cref="ArgumentException"></exception>
        protected BaseUser(string email,
            string phoneNumber, string surname, string name, string? patronymic = null)
        {
            if (string.IsNullOrWhiteSpace(surname)) throw new ArgumentException("Surname can't be null or whitespace", nameof(surname));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name can't be null or whitespace", nameof(name));
            UpdateEmail(email);
            UpdatePhoneNumber(phoneNumber);
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            FullName = string.Join(" ", Surname, Name, Patronymic?.Trim());
            NormalizedSurname = Surname.ToUpperInvariant();
            NormalizedName = Name.ToUpperInvariant();
            NormalizedPatronymic = Patronymic?.ToUpperInvariant() ?? string.Empty;
        }

        protected void UpdateEmail(string email)
        {
            ContactValidator.ValidateEmail(email);
            Email = email;
            NormalizedEmail = ContactValidator.NormalizeEmail(email);

        }

        protected void UpdatePhoneNumber(string phoneNumber)
        {
            ContactValidator.ValidatePhoneNumber(phoneNumber);
            PhoneNumber = phoneNumber;
            NormalizedPhoneNumber = ContactValidator.NormalizePhoneNumber(phoneNumber);
        }
    }
}
