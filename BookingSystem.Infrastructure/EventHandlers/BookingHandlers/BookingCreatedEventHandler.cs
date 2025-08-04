using BookingSystem.Core.Domain.Events;
using BookingSystem.Infrastructure.Services.Interfaces;
using MediatR;

namespace BookingSystem.Infrastructure.EventHandlers.BookingHandlers
{
    public class BookingCreatedEventHandler : INotificationHandler<MeetingCreatedEvent>
    {


        public BookingCreatedEventHandler(IMeetingService meetingRepository )
        {

        }

        public async Task Handle(MeetingCreatedEvent @event, CancellationToken cancellationToken)
        {

        }
    }
}