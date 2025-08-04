using BookingSystem.Core.Domain.Entities.Institutions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(e => e.Id);


            builder.Property(r => r.Number)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(r => r.CountOfPlaces)
                .IsRequired();

            builder.Property(r => r.IsBooked)
                .IsRequired();

            builder.Property(r => r.IsAvailable)
                .IsRequired();

            builder.HasOne(r => r.Building)
                .WithMany(b => b.Rooms)
                .HasForeignKey(r => r.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Schedule)
                .WithOne(s => s.Room)
                .HasForeignKey<Room>(r => r.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(r => r.Equipment, e =>
            {
                e.Property(eq => eq.HasProjector).HasColumnName("HasProjector");
                e.Property(eq => eq.HasSoundproofing).HasColumnName("HasSoundproofing");
                e.Property(eq => eq.HasWhiteboard).HasColumnName("HasWhiteboard");
                e.Property(eq => eq.HasInteractiveWhiteboard).HasColumnName("HasInteractiveWhiteboard");
                e.Property(eq => eq.NumberOfComputers).HasColumnName("NumberOfComputers");
                e.Property(eq => eq.HasVideoConferenceSystem).HasColumnName("HasVideoConferenceSystem");
                e.Property(eq => eq.HasMicrophones).HasColumnName("HasMicrophones");
                e.Property(eq => eq.NumberOfMicrophones).HasColumnName("NumberOfMicrophones");
                e.Property(eq => eq.HasAirConditioning).HasColumnName("HasAirConditioning");
                e.Property(eq => eq.HasTelevisions).HasColumnName("HasTelevisions");
                e.Property(eq => eq.NumberOfTelevisions).HasColumnName("NumberOfTelevisions");
                e.Property(eq => eq.HasWiFi).HasColumnName("HasWiFi");
            });

            builder.HasMany(r => r.Meetings)
                .WithOne(m => m.Room)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}