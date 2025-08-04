using BookingSystem.Core.Domain.Entities;
using BookingSystem.Core.Domain.Events;
using BookingSystem.Infrastructure.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.EventHandlers.UserHandlers
{
    public class UserRoleAssignedEventHandler : INotificationHandler<UserRoleAssignedEvent>
    {
        //private readonly IResourceAccessRepository _accessRepo;
        //private readonly IEmailService _emailService;

        public async Task Handle(UserRoleAssignedEvent @event, CancellationToken token)
        {

           /* var defaultAccess = ResourceAccessControl.Create(
                resourceId: Guid.NewGuid(), // Все ресурсы учреждения
                roleId: @event.RoleId,
                accessType: "ReadWrite"
            );
            await _accessRepo.AddAsync(defaultAccess);*/

           /* await _emailService.SendTemplateAsync(
                userId: @event.UserId,
                templateName: "NewRoleAssigned",
                parameters: new { EffectiveDate = @event.EffectiveDate }
            );*/
        }
    }
}
