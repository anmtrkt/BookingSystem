using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.InstitutionModels
{
    public class InstitutionDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required PriorityLevel PriorityLevel { get; init; }
        public InstitutionDto? Parent { get; init; }
    }
}
