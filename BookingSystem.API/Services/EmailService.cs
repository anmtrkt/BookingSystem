using BookingSystem.API.Services.Extensions;
using BookingSystem.Core.Domain.Models.MeetingsModels;
using BookingSystem.Infrastructure.Services.Interfaces;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> RegisterMail(string email, string name)
        {
            string header = $"Поздравляю, {name}!";
            string body = $"Вы успешно зарегистрировались!!!";
            var sended = _configuration.Sending(email, header, body);
            if (!sended)
                return await Task.FromResult(false);
            return await Task.FromResult(true);
        }
        public async Task<bool> BookingMail(string email, string name, MeetingDto response)
        {
            string header = $"Запланирована встреча";
            string body = $"{name}, вы назначили встречу в комнате {response.Room.Number} " +
                $"в {response.Institution.Name}, с {response.TimeRange.Start.TimeOfDay} " +
                $"по {response.TimeRange.End.TimeOfDay}";
            var sended = _configuration.Sending(email, header, body);
            if (!sended)
                return await Task.FromResult(false);
            return await Task.FromResult(true);
        }
        public async Task<bool> BookingReminderMail(string email, string name, MeetingDto response)
        {
            string header = $"Резервирование";
            string body = $"{name},напоминаем, что у вас запланирована встреча в {response.Room.Number} " +
                $"в {response.Institution.Name}, с {response.TimeRange.Start.TimeOfDay} " +
                $"по {response.TimeRange.End.TimeOfDay}" +
            $" До встречиосталось {(response.TimeRange.Start - DateTime.Now).TotalHours} дней";
            var sended = _configuration.Sending(email, header, body);
            if (!sended)
                return await Task.FromResult(false);
            return await Task.FromResult(true);
        }
        public async Task<bool> SubscribeToBookingMailAsync(string email, string name, MeetingDto response)
        {
            string header = $"Приглашение на встречу";
            string body = $"{name}, {response.Creator.FullName} приглашает вас на встречу, назначенную  " +
                $"в {response.Institution.Name}, с {response.TimeRange.Start.TimeOfDay} " +
                $"по {response.TimeRange.End.TimeOfDay}" +
            $" До встречи осталось {(response.TimeRange.Start - DateTime.Now).TotalHours} дней";
            var sended = _configuration.Sending(email, header, body);
            if (!sended)
                return await Task.FromResult(false);
            return await Task.FromResult(true);
        }
        public async Task<bool> SubscribedToBookingMail(string email, string name, MeetingDto response)
        {
            string header = $"Подписка на встречу";
            string body = $"{name}, вы подписались на встречу, назначенную  " +
                $"в {response.Institution.Name}, с {response.TimeRange.Start.TimeOfDay} " +
                $"по {response.TimeRange.End.TimeOfDay}" +
            $" До встречи осталось {(response.TimeRange.Start - DateTime.Now).TotalHours} дней";
            var sended = _configuration.Sending(email, header, body);
            if (!sended)
                return await Task.FromResult(false);
            return await Task.FromResult(true);
        }
        public async Task Reminder(DateOnly DateFrom, string email, string name, MeetingDto response)
        {
            var Today = DateTime.UtcNow;
            var reservedDay = DateFrom.ToDateTime(new TimeOnly());


            BackgroundJob.Schedule(
() => BookingReminderMail(email, name, response), reservedDay.AddHours(-24));
            BackgroundJob.Schedule(
() => BookingReminderMail(email, name, response), reservedDay.AddHours(-12));
            BackgroundJob.Schedule(
    () => BookingReminderMail(email, name, response), reservedDay.AddHours(-5));
            BackgroundJob.Schedule(
    () => BookingReminderMail(email, name, response), reservedDay.AddHours(-1));
            await Task.CompletedTask;

        }
    }
}
