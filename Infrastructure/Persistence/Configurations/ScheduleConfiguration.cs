using BookingSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;
public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(e => e.Id);

        builder.OwnsMany(s => s.TimeRanges, tr =>
        {
            tr.ToTable("ScheduleTimeRanges");
            tr.WithOwner().HasForeignKey("ScheduleId");

            tr.Property(t => t.Start).HasColumnName("StartTime");
            tr.Property(t => t.End).HasColumnName("EndTime");
        });
    }
}