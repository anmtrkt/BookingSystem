using BookingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingSystem.Application.Services;

public interface INotificationService
{
    // Создание уведомления (внутреннего и/или email)
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request);
    
    // Получение уведомлений пользователя
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId);
    
    // Пометить как прочитанное
    Task MarkAsReadAsync(Guid notificationId);
    
    // Пометить все как прочитанные
    Task MarkAllAsReadAsync(Guid userId);
    
    // Удалить уведомление
    Task DeleteNotificationAsync(Guid notificationId);
    
    // Отправить email о создании встречи
    Task SendMeetingCreatedEmailAsync(Guid meetingId, Guid recipientId);
    
    // Отправить напоминание о встрече
    Task SendMeetingReminderEmailAsync(Guid meetingId, Guid recipientId, TimeSpan reminderBefore);
    
    // Отправить приглашение на встречу
    Task SendMeetingInvitationEmailAsync(Guid invitationId);
    
    // Отправить уведомление об отказе от приглашения
    Task SendInvitationDeclinedEmailAsync(Guid invitationId);
    
    // Отправить уведомление о регистрации
    Task SendRegistrationEmailAsync(Guid userId);
}
