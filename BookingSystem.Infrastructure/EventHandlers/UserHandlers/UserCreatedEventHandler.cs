using BookingSystem.Core.Domain.Events.UserEvents;
using BookingSystem.Infrastructure.Services;
using BookingSystem.Infrastructure.Services.Interfaces;
using MediatR;

namespace BookingSystem.Infrastructure.EventHandlers.UserHandlers
{

    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly IUserService _userService;
        //private readonly IEmailService _emailService;

        public async Task Handle(UserCreatedEvent @event, CancellationToken token)
        {
            //await _emailService.SendWelcomeEmailAsync(@event.UserId);
        }
    }
}
