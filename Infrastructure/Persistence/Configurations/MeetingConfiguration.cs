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

        builder.OwnsOne(t => t.TimeRange)
                .Property(t => t.Start)
                .HasColumnName("StartTime")
                .IsRequired();

        builder.OwnsOne(t => t.TimeRange)
                .Property(t => t.Start)
                .HasColumnName("EndTime")
                .IsRequired();



        builder.HasOne(m => m.Creator)
               .WithMany()
               .HasForeignKey(m => m.CreatorId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}