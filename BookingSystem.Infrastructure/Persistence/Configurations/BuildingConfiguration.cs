using BookingSystem.Core.Domain.Entities.Institutions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");

        builder.HasKey(e => e.Id);

        builder.Property(b => b.Address)
            .IsRequired()
            .HasMaxLength(512);


        builder.HasOne(b => b.Branch)
            .WithMany(br => br.Buildings)
            .HasForeignKey(b => b.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Rooms)
            .WithOne(r => r.Building)
            .HasForeignKey(r => r.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}