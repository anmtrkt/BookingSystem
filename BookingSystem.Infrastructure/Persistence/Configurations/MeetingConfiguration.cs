using BookingSystem.Core.Domain.Entities.Aggregates;
using BookingSystem.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.ToTable("Meetings");
            builder.HasKey(e => e.Id);

            builder.Property(m => m.IsOverridden)
                .IsRequired();

            builder.Property(m => m.IsCancelled)
                .IsRequired();

            builder.OwnsOne(m => m.TimeRange, tr =>
            {
                tr.Property(t => t.Start)
                  .HasColumnName("StartTime") // Название столбца в БД
                  .IsRequired();

                tr.Property(t => t.End)
                  .HasColumnName("EndTime") // Название столбца в БД
                  .IsRequired();

            });

            // Игнорируем свойство TimeRange, так как используем StartTime и EndTime
        



            // Настройка отношения с Creator
            builder.HasOne(m => m.Creator)
                .WithMany(u => u.CreatedMeetings)
                .HasForeignKey(m => m.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка отношения с Room
            builder.HasOne(m => m.Room)
                .WithMany(r => r.Meetings)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Restrict);



/*            // Настройка индексов для ускорения запросов
            builder.HasIndex(m => m.RoomId);
            builder.HasIndex(m => m.CreatorId);
            builder.HasIndex(m => m.InstitutionId);*/
        }
    }
}