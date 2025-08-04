using BookingSystem.Core.Domain.Common;
using BookingSystem.Core.Domain.Entities;
using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Institutions;
using BookingSystem.Core.Domain.Entities.Notifications;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Core.Domain.ValueObjects;
using BookingSystem.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;


namespace BookingSystem.Infrastructure.Persistence
{
    public class BookingSystemDbContext : IdentityDbContext<User,                 // ваш класс пользователя
        Roles,                // ваш класс роли
        Guid,                 // тип ключа 
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,       // привязываем стандартный UserRole
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public BookingSystemDbContext(DbContextOptions<BookingSystemDbContext> options)
        : base(options)
        {
        }
        public DbSet<UserSubscribeRequest> UserSubscribeRequests { get; set; }
        public DbSet<Building> Buildings { get; set; }

        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
 

            modelBuilder.ApplyConfiguration(new UserSubscribeRequestConfiguration());
            modelBuilder.ApplyConfiguration(new BranchConfiguration());
            modelBuilder.ApplyConfiguration(new BuildingConfiguration());
            modelBuilder.ApplyConfiguration(new InstitutionConfiguration());

            modelBuilder.ApplyConfiguration(new MeetingConfiguration());
           modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.ApplyConfiguration(new NotificationConfiguration());


            base.OnModelCreating(modelBuilder);
        }


    }
}