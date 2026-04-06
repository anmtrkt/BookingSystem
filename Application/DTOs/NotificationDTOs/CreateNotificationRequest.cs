namespace BookingSystem.Application.DTOs;

public enum NotificationType
{
    MeetingCreated,      // Уведомление о создании встречи
    MeetingReminder,     // Напоминание о встрече
    MeetingInvitation,   // Приглашение на встречу
    InvitationDeclined,  // Отказ от приглашения
    InvitationAccepted,  // Принятие приглашения
    Registration         // Уведомление о регистрации
}

public class CreateNotificationRequest
{
    public required Guid ReceiverId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required string Sender { get; set; }
    public NotificationType Type { get; set; } = NotificationType.MeetingCreated;
    public bool SendEmail { get; set; } = true;
    public Guid? MeetingId { get; set; }
    public DateTime? ReminderTime { get; set; }
}
