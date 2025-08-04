using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Entities.Notifications
{
    public class Notification : BaseEntity
    {
        public string Title { get; private set; }
        public string Body { get; private set; }
        public Guid ReceiverId { get; init; }
        public bool isRead { get; private set; } = false;
        public NotificationType Type { get; init; }
        public User Receiver {  get; init; }

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Notification() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private Notification(User receiver, string title, string body)
        {
            Title = title;
            Body = body;
            ReceiverId = receiver.Id;
            Receiver = receiver;
        }
        public static Notification Create(User receiver, string title, string body)
        {
            return new Notification(receiver, title, body);
        }
        public void MarkAsRead()
        {
            isRead = true;
            MarkAsModified();
        }
        public void MarkAsUnread()
        {
            isRead = false;
            MarkAsModified();
        }
        
        public override string ToString()
        {
            return $"[{Type.ToString()}] ::: {Title}: {Body}; Sended at {CreatedAt} to {Receiver.FullName}({ReceiverId})";
        }
        
    }
    public enum NotificationType
    {
        LowPriority,
        MidPriority,
        HighPriority
    }
}
