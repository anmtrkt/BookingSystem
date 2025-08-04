using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.Models.InstitutionModels;
using BookingSystem.Core.Domain.Models.RoomModels;
using BookingSystem.Core.Domain.Models.UserModels;
using BookingSystem.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.MeetingsModels
{
    public class MeetingDto
    {
        public required Guid Id { get; set; }
        public required TimeRange TimeRange { get; init; }
        public required bool IsCancelled { get; init; }

        public required InstitutionDto Institution { get; init; }
        public required UserDto Creator { get; init; }
        public required RoomDto Room { get;init; }
        public required ICollection<UserDto> Subscribers { get; init; }

    }
}
