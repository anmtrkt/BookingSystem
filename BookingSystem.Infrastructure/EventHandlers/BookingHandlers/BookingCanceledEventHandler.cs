using BookingSystem.Core.Domain.Events;
using MediatR;

public class BookingCanceledEventHandler : INotificationHandler<MeetingCanceledEvent>
{
    //конструктор
    public async Task Handle(MeetingCanceledEvent @event, CancellationToken token)
    {
        //send on email
       
    }
}