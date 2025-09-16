using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Models.InstitutionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.UserModels
{
    public class UserDto
    {
        public required string Email { get; set; }
        public required Guid Id { get; set; }
        public required DateTime CreatedAt { get; init; } 
        public required DateTime? ModifiedAt { get; init; }
        public required string Post { get; init; }
        public required string LastPost { get; init; }
        public required string FullName { get; init; }
        public required bool IsManager { get; init; }

    }
}
