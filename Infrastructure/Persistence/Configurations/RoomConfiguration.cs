using BookingSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;
public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Number).IsRequired().HasMaxLength(50);
        builder.Property(e => e.IsAvailable).HasDefaultValue(true);
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
    }
}