using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Core.Domain.ValueObjects
{
    [Owned]
    public class TimeRange : ValueObject
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        private TimeRange() { } // Для сериализации

        private TimeRange(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("End should be later than start");
            if (end - start > TimeSpan.FromHours(24))
                throw new ArgumentException("Max duration can't be more than 24h");

            Start = start;
            End = end;
        }

        public static TimeRange Create(DateTime start, DateTime end)
        {
            return new TimeRange(start, end);
        }

        public bool IsOverlapping(ref TimeRange other)
        {
            return (Start < other.End) && (End > other.Start);
        }

        public TimeSpan Duration => End - Start;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}