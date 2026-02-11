using BookingSystem.Domain.Interfaces;
using BookingSystem.Infrastructure.Repositories;
using BookingSystem.Infrastructure.Repositories.UnitOfWork;
using Infrastructure.Repositories.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace BookingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        return services;
    }
}