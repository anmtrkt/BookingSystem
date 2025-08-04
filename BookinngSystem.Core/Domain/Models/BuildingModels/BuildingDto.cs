using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Models.BranchModels;
using BookingSystem.Core.Domain.Models.RoomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.BuildingModels
{
    public class BuildingDto
    {
        public required Guid Id { get; init; }
        public required string Address { get; init; }
        public required BranchDto Branch { get; init; }

        public required List<RoomDto> Rooms {  get; init; }
    }
}
