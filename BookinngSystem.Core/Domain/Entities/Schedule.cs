using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.ValueObjects;

namespace BookingSystem.Core.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        private readonly List<TimeRange> _timeRanges = new();
        public IReadOnlyCollection<TimeRange> TimeRanges => _timeRanges.AsReadOnly();
        public Guid RoomId { get; private set; }
        public Room Room { get; private set; }

        public string Reason { get; private set; }

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Schedule() { } // Для сериализации
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private Schedule(
            Room room,
            Guid roomId)
        {
            Reason = "";
            Room = room;
            RoomId = roomId;
           
        }


        /// <exception cref="ArgumentException"></exception>
        public static Schedule Create(
            Room room)
        {
            
            
            return new Schedule(room,room.Id);
        }
        public void AddTime(DateTime start, DateTime end, Meeting meeting)
        {
            _timeRanges.Add(TimeRange.Create(start, end, meeting));
        }
        public void AddTime(TimeRange meetingTime)
        {
            _timeRanges.Add(meetingTime);
        }
        /*        public bool IsWithinSchedule(DateTime date)
                {
                    return date >= StartDate && date <= EndDate;
                }*/
    }
}