using BookingSystem.Core.Domain.Events;
using BookingSystem.Infrastructure.Services.Interfaces;
using MediatR;

namespace BookingSystem.Infrastructure.EventHandlers.RoomHandlers
{
    public class RoomEquipmentUpdatedEventHandler : INotificationHandler<RoomEquipmentUpdatedEvent>
    {
        private readonly IRoomService _roomService;

        public async Task Handle(RoomEquipmentUpdatedEvent @event, CancellationToken token)
        {


        }
    }
}
