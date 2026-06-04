using BookingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RoomDTOs
{
    public class FilterRoomDto
    {

            public bool? HasProjector { get; set; }
            public bool? HasSoundproofing { get; set; }
            public bool? HasWhiteboard { get; set; }
            public bool? HasInteractiveWhiteboard { get; set; }
            public int? MinComputers { get; set; }
            public bool? HasVideoConferenceSystem { get; set; }
            public bool? HasMicrophones { get; set; }
            public int? MinMicrophones { get; set; }
            public bool? HasAirConditioning { get; set; }
            public bool? HasTelevisions { get; set; }
            public int? MinTelevisions { get; set; }
            public bool? HasWiFi { get; set; }
        }

    public static class RoomQueryExtensions
    {
        public static IQueryable<Room> ApplyFilter(this IQueryable<Room> query, FilterRoomDto filter)
        {
            if (filter.HasProjector.HasValue)
                query = query.Where(c => c.Equipment.HasProjector == filter.HasProjector.Value);

            if (filter.HasSoundproofing.HasValue)
                query = query.Where(c => c.Equipment.HasSoundproofing == filter.HasSoundproofing.Value);

            if (filter.HasWhiteboard.HasValue)
                query = query.Where(c => c.Equipment.HasWhiteboard == filter.HasWhiteboard.Value);

            if (filter.HasInteractiveWhiteboard.HasValue)
                query = query.Where(c => c.Equipment.HasInteractiveWhiteboard == filter.HasInteractiveWhiteboard.Value);

            if (filter.MinComputers.HasValue)
                query = query.Where(c => c.Equipment.NumberOfComputers >= filter.MinComputers.Value);

            if (filter.HasVideoConferenceSystem.HasValue)
                query = query.Where(c => c.Equipment.HasVideoConferenceSystem == filter.HasVideoConferenceSystem.Value);

            if (filter.HasMicrophones.HasValue)
                query = query.Where(c => c.Equipment.HasMicrophones == filter.HasMicrophones.Value);

            if (filter.MinMicrophones.HasValue)
                query = query.Where(c => c.Equipment.NumberOfMicrophones >= filter.MinMicrophones.Value);

            if (filter.HasAirConditioning.HasValue)
                query = query.Where(c => c.Equipment.HasAirConditioning == filter.HasAirConditioning.Value);

            if (filter.HasTelevisions.HasValue)
                query = query.Where(c => c.Equipment.HasTelevisions == filter.HasTelevisions.Value);

            if (filter.MinTelevisions.HasValue)
                query = query.Where(c => c.Equipment.NumberOfTelevisions >= filter.MinTelevisions.Value);

            if (filter.HasWiFi.HasValue)
                query = query.Where(c => c.Equipment.HasWiFi == filter.HasWiFi.Value);

            return query;
        }
    }

}
