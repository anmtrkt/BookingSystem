using BookingSystem.Core.Entities;
using BookingSystem.Core.Entities.Aggregates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence;

public class BookingSystemDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public BookingSystemDbContext(DbContextOptions<BookingSystemDbContext> options) : base(options)
    {

    }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(BookingSystemDbContext).Assembly);
    }
}