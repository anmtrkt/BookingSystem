using BookingSystem.Core.Domain.Models.MeetingsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SubscribeToBookingMailAsync(string email, string name, MeetingDto response);
        public Task<bool> SubscribedToBookingMail(string email, string name, MeetingDto response);
        public Task<bool> RegisterMail(string email, string name);
        public Task<bool> BookingMail(string email, string name, MeetingDto response);
        public Task<bool> BookingReminderMail(string email, string name, MeetingDto response);
        public Task Reminder(DateOnly DateFrom, string email, string name, MeetingDto response);
    }
}
