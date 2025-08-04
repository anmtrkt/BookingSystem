using BookingSystem.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("Schedules");
            builder.HasKey(e => e.Id);


            builder.Property(s => s.Reason)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasOne(s => s.Room)
                .WithOne(r => r.Schedule)
                .HasForeignKey<Schedule>(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(s => s.TimeRanges, tr =>
            {
                tr.ToTable("ScheduleTimeRanges");
                tr.WithOwner().HasForeignKey("ScheduleId");

                tr.Property(t => t.Start).HasColumnName("StartTime");
                tr.Property(t => t.End).HasColumnName("EndTime");
            });
        }
    }
}