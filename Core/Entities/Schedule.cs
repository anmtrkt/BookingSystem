using BookingSystem.Core.ValueObjects;

namespace BookingSystem.Core.Entities;

public class Schedule : BaseEntity
{

    public ICollection<TimeRange> TimeRanges { get; init; } = new HashSet<TimeRange>();

    public Schedule()
    {
        TimeRanges = new List<TimeRange>();
    }
    public void AddTime(DateTime start, DateTime end)
    {
        TimeRange range = new TimeRange(start, end);
        TimeRanges.Add(range);
    }
    public void RemoveTime(TimeRange range)
    {
        TimeRanges.Remove(range);
    }

}
