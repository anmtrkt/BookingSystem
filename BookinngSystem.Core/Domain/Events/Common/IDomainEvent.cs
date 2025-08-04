using MediatR;

namespace BookingSystem.Core.Domain.Events
{
    public interface IDomainEvent:INotification
    {
        DateTime OccurredOn { get; }


    }
}