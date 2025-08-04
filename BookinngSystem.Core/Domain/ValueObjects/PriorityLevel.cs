namespace BookingSystem.Core.Domain.ValueObjects
{
    public class PriorityLevel : ValueObject
    {
        private const byte MinValue = 1;
        private const byte MaxValue = 10;

        public byte Level { get; private set; }

        private PriorityLevel() { } // Для сериализации

        private PriorityLevel(byte level)
        {
            if (level < MinValue || level > MaxValue)
                throw new ArgumentOutOfRangeException(
                    nameof(level),
                    $"Priority must be in [{MinValue}, {MaxValue}]."
                );

            Level = MinValue;
        }

        public static PriorityLevel Create(byte level)
        {
            return new PriorityLevel(level);
        }

        public bool IsHigherThan(PriorityLevel other)
        {
            return Level > other.Level;
        }

        public static PriorityLevel Highest => Create(MaxValue);
        public static PriorityLevel Lowest => Create(MinValue);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Level;
        }
    }
}