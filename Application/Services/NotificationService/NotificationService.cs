using BookingSystem.Application.DTOs;
using BookingSystem.Core.Entities.Aggregates;
using BookingSystem.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace BookingSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;

    private readonly string? _smtpServer;
    private readonly int _smtpPort;
    private readonly string? _smtpUsername;
    private readonly string? _smtpPassword;
    private readonly string? _smtpFromEmail;
    private readonly bool _enableSmtp;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IBookingRepository bookingRepository,
        IInvitationRepository invitationRepository,
        IRoomRepository roomRepository,
        IConfiguration configuration,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _invitationRepository = invitationRepository;
        _roomRepository = roomRepository;
        _configuration = configuration;
        _logger = logger;

        // Чтение настроек SMTP из конфигурации
        _smtpServer = _configuration["Smtp:Server"];
        _smtpPort = _configuration.GetValue<int>("Smtp:Port", 587);
        _smtpUsername = _configuration["Smtp:Username"];
        _smtpPassword = _configuration["Smtp:Password"];
        _smtpFromEmail = _configuration["Smtp:FromEmail"];
        _enableSmtp = _configuration.GetValue<bool>("Smtp:Enable", false);
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request)
    {
        _logger.LogInformation("Creating notification for user {UserId} of type {Type}", request.ReceiverId, request.Type);

        var receiver = await _userRepository.GetByIdAsync(request.ReceiverId);
        if (receiver == null)
            throw new KeyNotFoundException($"User with ID {request.ReceiverId} not found.");

        var notification = new Notification(receiver, request.Sender, request.Title, request.Body);

        await _notificationRepository.AddAsync(notification);
        
        // Сохранение будет выполнено в UnitOfWork вызывающей стороны

        if (request.SendEmail && _enableSmtp)
        {
            await SendEmailAsync(receiver.Email!, request.Title, request.Body);
        }

        _logger.LogInformation("Notification created successfully for user {UserId}", request.ReceiverId);

        return MapToDto(notification);
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId)
    {
        _logger.LogInformation("Getting notifications for user {UserId}", userId);

        var notifications = await _notificationRepository.GetAllAsyncWithInclude();
        var userNotifications = notifications.Where(n => n.ReceiverId == userId)
            .OrderByDescending(n => n.WhenSended);

        return userNotifications.Select(MapToDto);
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        _logger.LogInformation("Marking notification {NotificationId} as read", notificationId);

        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
            throw new KeyNotFoundException($"Notification with ID {notificationId} not found.");

        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification);
        
        _logger.LogInformation("Notification {NotificationId} marked as read", notificationId);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        _logger.LogInformation("Marking all notifications as read for user {UserId}", userId);

        var notifications = await _notificationRepository.GetAllAsyncWithInclude();
        var userNotifications = notifications.Where(n => n.ReceiverId == userId && !n.IsRead).ToList();

        foreach (var notification in userNotifications)
        {
            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);
        }

        _logger.LogInformation("All notifications marked as read for user {UserId}", userId);
    }

    public async Task DeleteNotificationAsync(Guid notificationId)
    {
        _logger.LogInformation("Deleting notification {NotificationId}", notificationId);

        await _notificationRepository.DeleteAsync(notificationId);
        
        _logger.LogInformation("Notification {NotificationId} deleted", notificationId);
    }

    public async Task SendMeetingCreatedEmailAsync(Guid meetingId, Guid recipientId)
    {
        _logger.LogInformation("Sending meeting created email for meeting {MeetingId} to user {UserId}", meetingId, recipientId);

        var meeting = await _bookingRepository.GetByIdAsyncWithInclude(meetingId);
        if (meeting == null)
            throw new KeyNotFoundException($"Meeting with ID {meetingId} not found.");

        var room = await _roomRepository.GetByIdAsyncWithInclude(meeting.RoomId);

        var recipient = await _userRepository.GetByIdAsync(recipientId);
        if (recipient == null)
            throw new KeyNotFoundException($"User with ID {recipientId} not found.");

        var subject = $"Встреча создана: {meeting.Reason}";
        var body = $@"
<html>
<body>
    <h2>Уведомление о создании встречи</h2>
    <p>Здравствуйте, {recipient.Name}!</p>
    <p>Была создана новая встреча:</p>
    <ul>
        <li><strong>Тема:</strong> {meeting.Reason}</li>
        <li><strong>Время начала:</strong> {meeting.TimeRange.Start:dd.MM.yyyy HH:mm}</li>
        <li><strong>Время окончания:</strong> {meeting.TimeRange.End:dd.MM.yyyy HH:mm}</li>
        <li><strong>Место:</strong> {meeting.Room.Number} (Офис: {room.Office.Address})</li>
        <li><strong>Организатор:</strong> {meeting.Creator.FullName}</li>
    </ul>
</body>
</html>";

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = recipientId,
            Title = "Встреча создана",
            Body = $"Встреча '{meeting.Reason}' создана на {meeting.TimeRange.Start:dd.MM.yyyy HH:mm}",
            Sender = "Система бронирования",
            Type = NotificationType.MeetingCreated,
            SendEmail = true,
            MeetingId = meetingId
        });
        await SendEmailAsync(recipient.Email, subject, body);

        _logger.LogInformation("Meeting created email sent to user {UserId}", recipientId);
    }

    public async Task SendMeetingReminderEmailAsync(Guid meetingId, Guid recipientId, TimeSpan reminderBefore)
    {
        _logger.LogInformation("Sending meeting reminder for meeting {MeetingId} to user {UserId}", meetingId, recipientId);

        var meeting = await _bookingRepository.GetByIdAsyncWithInclude(meetingId);
        if (meeting == null)
            throw new KeyNotFoundException($"Meeting with ID {meetingId} not found.");

        if (meeting.IsCancelled)
        {
            _logger.LogWarning("Meeting {MeetingId} is cancelled, skipping reminder", meetingId);
            return;
        }

        var recipient = await _userRepository.GetByIdAsync(recipientId);
        if (recipient == null)
            throw new KeyNotFoundException($"User with ID {recipientId} not found.");

        var timeUntilMeeting = meeting.TimeRange.Start - DateTime.UtcNow;
        var reminderText = FormatTimeSpan(reminderBefore);

        var subject = $"Напоминание о встрече: {meeting.Reason}";
        var body = $@"
<html>
<body>
    <h2>Напоминание о предстоящей встрече</h2>
    <p>Здравствуйте, {recipient.Name}!</p>
    <p>Напоминаем вам о предстоящей встрече через {reminderText}:</p>
    <ul>
        <li><strong>Тема:</strong> {meeting.Reason}</li>
        <li><strong>Время начала:</strong> {meeting.TimeRange.Start:dd.MM.yyyy HH:mm}</li>
        <li><strong>Время окончания:</strong> {meeting.TimeRange.End:dd.MM.yyyy HH:mm}</li>
        <li><strong>Место:</strong> {meeting.Room.Number} (Офис: {meeting.Room.Office.Address})</li>
        <li><strong>Организатор:</strong> {meeting.Creator.FullName}</li>
    </ul>
</body>
</html>";

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = recipientId,
            Title = $"Напоминание: встреча через {reminderText}",
            Body = $"Встреча '{meeting.Reason}' начнется через {reminderText}",
            Sender = "Система бронирования",
            Type = NotificationType.MeetingReminder,
            SendEmail = true,
            MeetingId = meetingId,
            ReminderTime = meeting.TimeRange.Start - reminderBefore
        });
        await SendEmailAsync(recipient.Email, subject, body);

        _logger.LogInformation("Meeting reminder sent to user {UserId}", recipientId);
    }

    public async Task SendMeetingInvitationEmailAsync(Guid invitationId)
    {
        _logger.LogInformation("Sending meeting invitation for invitation {InvitationId}", invitationId);

        var invitation = await _invitationRepository.GetByIdAsyncWithInclude(invitationId);
        if (invitation == null)
            throw new KeyNotFoundException($"Invitation with ID {invitationId} not found.");

        var meeting = invitation.Meeting;
        var invitee = invitation.Invitee;
        var inviter = invitation.Inviter;

        var subject = $"Приглашение на встречу: {meeting.Reason}";
        var body = $@"
<html>
<body>
    <h2>Приглашение на встречу</h2>
    <p>Здравствуйте, {invitee.Name}!</p>
    <p>Пользователь {inviter.FullName} приглашает вас на встречу:</p>
    <ul>
        <li><strong>Тема:</strong> {meeting.Reason}</li>
        <li><strong>Время начала:</strong> {meeting.TimeRange.Start:dd.MM.yyyy HH:mm}</li>
        <li><strong>Время окончания:</strong> {meeting.TimeRange.End:dd.MM.yyyy HH:mm}</li>
        <li><strong>Место:</strong> {meeting.Room.Number} (Офис: {meeting.Room.Office.Address})</li>
    </ul>
    <p>Пожалуйста, подтвердите или отклоните приглашение.</p>
</body>
</html>";

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = invitation.InviteeId,
            Title = "Приглашение на встречу",
            Body = $"{inviter.FullName} приглашает вас на встречу '{meeting.Reason}'",
            Sender = inviter.FullName,
            Type = NotificationType.MeetingInvitation,
            SendEmail = true,
            MeetingId = meeting.Id
        });
        await SendEmailAsync(invitee.Email, subject, body);

        _logger.LogInformation("Meeting invitation sent to user {UserId}", invitation.InviteeId);
    }

    public async Task SendInvitationDeclinedEmailAsync(Guid invitationId)
    {
        _logger.LogInformation("Sending invitation declined notification for invitation {InvitationId}", invitationId);

        var invitation = await _invitationRepository.GetByIdAsyncWithInclude(invitationId);
        if (invitation == null)
            throw new KeyNotFoundException($"Invitation with ID {invitationId} not found.");

        var meeting = invitation.Meeting;
        var invitee = invitation.Invitee;
        var inviter = await _userRepository.GetByIdAsync(invitation.InviterId);
        
        if (inviter == null)
            throw new KeyNotFoundException($"Inviter with ID {invitation.InviterId} not found.");

        var subject = $"Приглашение отклонено: {meeting.Reason}";
        var body = $@"
<html>
<body>
    <h2>Приглашение отклонено</h2>
    <p>Здравствуйте, {inviter.Name}!</p>
    <p>Пользователь {invitee.FullName} отклонил ваше приглашение на встречу:</p>
    <ul>
        <li><strong>Тема:</strong> {meeting.Reason}</li>
        <li><strong>Время начала:</strong> {meeting.TimeRange.Start:dd.MM.yyyy HH:mm}</li>
        <li><strong>Место:</strong> {meeting.Room.Number}</li>
    </ul>
</body>
</html>";

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = invitation.InviterId,
            Title = "Приглашение отклонено",
            Body = $"{invitee.FullName} отклонил приглашение на встречу '{meeting.Reason}'",
            Sender = "Система бронирования",
            Type = NotificationType.InvitationDeclined,
            SendEmail = true,
            MeetingId = meeting.Id
        });
        await SendEmailAsync(inviter.Email, subject, body);

        _logger.LogInformation("Invitation declined notification sent to user {UserId}", invitation.InviterId);
    }

    public async Task SendRegistrationEmailAsync(Guid userId)
    {
        _logger.LogInformation("Sending registration email to user {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        var subject = "Добро пожаловать в систему бронирования переговорных комнат!";
        var body = $@"
<html>
<body>
    <h2>Добро пожаловать!</h2>
    <p>Здравствуйте, {user.FullName}!</p>
    <p>Вы успешно зарегистрировались в системе бронирования переговорных комнат.</p>
    <p>Теперь вы можете:</p>
    <ul>
        <li>Бронировать переговорные комнаты</li>
        <li>Создавать встречи и приглашать коллег</li>
        <li>Получать уведомления о предстоящих встречах</li>
        <li>Управлять своими бронированиями</li>
    </ul>
    <p>Для входа в систему используйте ваш email: {user.Email}</p>
</body>
</html>";

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = userId,
            Title = "Добро пожаловать!",
            Body = "Вы успешно зарегистрировались в системе бронирования",
            Sender = "Система бронирования",
            Type = NotificationType.Registration,
            SendEmail = true
        });
        await SendEmailAsync(user.Email, subject, body);
        _logger.LogInformation("Registration email sent to user {UserId}", userId);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrEmpty(_smtpServer) || string.IsNullOrEmpty(_smtpUsername) || 
            string.IsNullOrEmpty(_smtpPassword) || string.IsNullOrEmpty(_smtpFromEmail))
        {
            _logger.LogWarning("SMTP settings are not configured. Email not sent.");
            return;
        }

        try
        {
            //using var client = new SmtpClient(_smtpServer, _smtpPort)
            //{
            //    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
            //    EnableSsl = true
            //};


            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("anmtrkt2@gmail.com", "ijma nyni njiq pvuz"),
                EnableSsl = true, 
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpFromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}. Error: {Error}", toEmail, ex.Message);
            throw;
        }
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
        {
            var days = (int)timeSpan.TotalDays;
            return $"{days} дн.";
        }
        if (timeSpan.TotalHours >= 1)
        {
            var hours = (int)timeSpan.TotalHours;
            return $"{hours} ч.";
        }
        var minutes = (int)timeSpan.TotalMinutes;
        return $"{minutes} мин.";
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Title = notification.Title,
            Description = notification.Body,
            Created = notification.WhenSended,
            ReceiverId = notification.ReceiverId,
            Sender = notification.Sender,
            IsReaded = notification.IsRead
        };
    }
}