using BookingSystem.Core.Domain.Events;
using BookingSystem.Infrastructure.Services;
using MediatR;

public class BookingOverriddenEventHandler : INotificationHandler<BookingOverriddenEvent>
{
    //private readonly IEmailService _emailService;
    //private readonly IInstitutionRepository _institutionRepo;

    public async Task Handle(BookingOverriddenEvent @event, CancellationToken token)
    {

    }
}