using BookingSystem.Core.Entities;
using BookingSystem.Core.Entities.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BookingSystem.Infrastructure.Data.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsCancelled).HasDefaultValue(false);
        builder.HasOne(m => m.Room)
               .WithMany()
               .HasForeignKey(m => m.RoomId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsOne(t => t.TimeRange, tr =>
        {
            tr.Property(t => t.Start).HasColumnName("StartTime").IsRequired();
            tr.Property(t => t.End).HasColumnName("EndTime").IsRequired();
        });


        builder.HasOne(m => m.Creator)
               .WithMany()
               .HasForeignKey(m => m.CreatorId)
               .OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(m => m.Subscribers)
       .WithMany()
       .UsingEntity<Dictionary<string, object>>(
           "MeetingSubscribers",  // имя промежуточной таблицы
           j => j.HasOne<AppUser>()
                 .WithMany()
                 .HasForeignKey("UserId")
                 .OnDelete(DeleteBehavior.Cascade),
           j => j.HasOne<Meeting>()
                 .WithMany()
                 .HasForeignKey("MeetingId")
                 .OnDelete(DeleteBehavior.Cascade),
           j =>
           {
               j.HasKey("UserId", "MeetingId");
           });
    }
}