using BookingSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        return services;
    }
}