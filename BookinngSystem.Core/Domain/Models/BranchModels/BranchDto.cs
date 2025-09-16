using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.BuildingModels;
using BookingSystem.Core.Domain.Models.InstitutionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.BranchModels
{
    public class BranchDto
    {
        public required Guid Id { get; init; }
        public required InstitutionDto Institution { get; init; }
      
        public required List<BuildingDto> Buildings { get; init; }
        public required string Name { get; init; }
        public required string Address { get; init; }

    }
}
