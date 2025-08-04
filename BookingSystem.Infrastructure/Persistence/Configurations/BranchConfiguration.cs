using BookingSystem.Core.Domain.Entities.Institutions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(e => e.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(b => b.Address)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasOne(b => b.Institution)
                .WithMany(i => i.Branches)
                .HasForeignKey(b => b.InstitutionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Buildings)
                .WithOne(b => b.Branch)
                .HasForeignKey(b => b.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}