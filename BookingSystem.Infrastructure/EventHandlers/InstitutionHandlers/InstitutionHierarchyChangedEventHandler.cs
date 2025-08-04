using BookingSystem.Core.Domain.Events;
using MediatR;

namespace BookingSystem.Infrastructure.EventHandlers.InstitutionHandlers
{
    public class InstitutionHierarchyChangedEventHandler : INotificationHandler<InstitutionHierarchyChangedEvent>
    {
       //s private readonly IInstitutionRepository _institutionRepo;


        public async Task Handle(InstitutionHierarchyChangedEvent @event, CancellationToken token)
        {

        }
    }
}
