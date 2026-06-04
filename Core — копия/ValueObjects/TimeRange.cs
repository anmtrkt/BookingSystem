using BookingSystem.Core.Entities;

namespace BookingSystem.Core.ValueObjects;
public class TimeRange : IEquatable<TimeRange>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public TimeRange() { }
    public TimeRange(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new ArgumentException("End should be later than start");
        if (end - start > TimeSpan.FromHours(24))
            throw new ArgumentException("Max duration can't be more than 24h");
        Start = start;
        End = end;
    }
    public bool IsOverlapping(in TimeRange other) => Start < other.End && End > other.Start;
    public bool IsOverlapping(DateTime start, DateTime end) => Start < end && End > start;
    public TimeSpan Duration => End - Start;
    public bool Equals(TimeRange? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Start == other.Start && End == other.End;
    }


    public override bool Equals(object? obj)
    {
        return Equals(obj as TimeRange);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }
    public static bool operator ==(TimeRange? left, TimeRange? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }

    public static bool operator !=(TimeRange? left, TimeRange? right)
    {
        return !(left == right);
    }
}