namespace BookingSystem.Core.Entities.Aggregates
{
    public class Notification : BaseEntity
    {
        public string Title { get; init; }
        public string Body { get; init; }
        public AppUser Receiver { get; init; }
        public Guid ReceiverId { get; init; }
        public string Sender { get; init; }
        public DateTime WhenSended { get; init; }
        public bool IsRead { get; private set; } = false;
#pragma warning disable CS8618
        public Notification() { }
#pragma warning restore CS8618 
        public Notification(AppUser receiver, string sender, string title, string body)
        {
            Title = title;
            Body = body;
            Receiver = receiver;
            ReceiverId = receiver.Id;
            Sender = sender;
        }
        public void MarkAsRead() => IsRead = true;
        public void MarkAsUnread() => IsRead = false;
    }
}
