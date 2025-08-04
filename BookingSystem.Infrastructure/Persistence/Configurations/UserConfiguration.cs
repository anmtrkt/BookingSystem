using BookingSystem.Core.Domain.Entities;
using BookingSystem.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(u => u.Surname)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(u => u.Patronymic)
                .HasMaxLength(128);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Post)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(u => u.LastPost)
                .HasMaxLength(128);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.ModifiedAt);

            builder.Property(u => u.IsArchive)
                .IsRequired();

            builder.HasOne(u => u.Institution)
                .WithMany(i => i.Employees)
                .HasForeignKey(u => u.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(u => u.ManagerUsersId);
            builder.Ignore(u => u.CreatedMeetingsId);

            builder.HasMany(u => u.ManagedUsers)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserManagers"));

            builder.HasMany(u => u.CreatedMeetings)
                .WithOne(m => m.Creator)
                .HasForeignKey(m => m.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}