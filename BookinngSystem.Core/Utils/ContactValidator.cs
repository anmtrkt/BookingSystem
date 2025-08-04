// Ignore Spelling: Utils

using System.Text.RegularExpressions;

namespace BookingSystem.Core.Utils
{
    public static class ContactValidator
    {

        private const string EmailPattern = @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}";

        private const string PhoneNumberPattern = @"^\+?(\d[\d\-\.\s$$]{7,}\d)$";


        /// <param name="email"></param>
        /// <exception cref="ArgumentException"></exception>
        public static string ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email can't be null or whitespace", nameof(email));

            if (!Regex.IsMatch(email, EmailPattern))
                throw new ArgumentException("Invalid email format", nameof(email));
            return NormalizeEmail(email);
        }

        public static string NormalizeEmail(string email)
        {
            return email.ToUpperInvariant();
        }


        /// <param name="phoneNumber"></param>
        /// <exception cref="ArgumentException"></exception>
        public static string ValidatePhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number can't be null or whitespace", nameof(phoneNumber));

            if (!Regex.IsMatch(phoneNumber, PhoneNumberPattern))
                throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));
            return phoneNumber;
        }


    }
}