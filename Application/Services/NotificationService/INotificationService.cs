using BookingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingSystem.Application.Services;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request);
    Task DeleteOfficeAsync(Guid id);
    Task<NotificationDto> GetOfficeByIdAsync(Guid id);
    Task<IEnumerable<NotificationDto>> GetAllOfficesAsync();
}
