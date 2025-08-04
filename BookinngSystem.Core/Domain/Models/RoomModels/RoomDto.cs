using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Models.RoomModels
{
    public class RoomDto
    {
        public required Guid Id { get; init; }
        public required string Number { get; init; }
        public required Building Building { get; init; }

        public required Equipment Equipment { get; init; }
        public required bool IsBooked { get; init; }
        public required bool IsAvailable { get; init; }
        public required uint CountOfPlaces { get; init; }
        public required Schedule Schedule { get; init; }

    }
}
