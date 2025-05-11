using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Common
{
    class BaseUser : BaseEntity
    {
        public string Surname { get; private set; }
        public string Name { get; private set; }
        public string? Patronymic { get; private set; }
        public string FullName { get; private set; }
        public string NormSurname { get; private set; }
        public string NormName { get; private set; }
        public string? NormPatronymic { get; private set; }
        public string NormFullName { get; private set; }
        public 


        protected BaseUser(string surname, string name, string patronymic = null) {
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            //test when null patronymic
            string.Join(" ", surname, name, patronymic?.Trim());
            NormSurname = Surname.ToUpper();
            NormName = Name.ToUpper();
            NormPatronymic = Patronymic?.ToUpper() ?? string.Empty;
            NormFullName = FullName.ToUpper();
        }
    }
}
