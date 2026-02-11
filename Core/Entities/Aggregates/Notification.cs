namespace BookingSystem.Core.Entities.Aggregates
{
    public class Notification : BaseEntity
    {
        public string Title { get; init; }
        public string Body { get; init; }
        public User Receiver { get; init; }
        public bool IsRead { get; private set; } = false;
#pragma warning disable CS8618
        public Notification() { }
#pragma warning restore CS8618 
        public Notification(User receiver, string title, string body)
        {
            Title = title;
            Body = body;
            Receiver = receiver;
        }
        public void MarkAsRead() => IsRead = true;
        public void MarkAsUnread() => IsRead = false;
    }
}
