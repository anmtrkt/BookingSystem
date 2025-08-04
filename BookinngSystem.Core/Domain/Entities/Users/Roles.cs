
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Entities.Users
{
    public class Roles : IdentityRole<Guid>
    {
        /// <summary>
        /// AppAdmin
        /// </summary>
        public const string Admin = "admin";
        public const string User = "user";
        public const string Manager = "manager";
        /// <summary>
        /// InstAddmin
        /// </summary>
        public const string Admininstration = "admininstration";

    }
}
