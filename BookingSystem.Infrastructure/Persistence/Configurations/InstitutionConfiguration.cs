using BookingSystem.Core.Domain.Entities.Institutions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
    {
        public void Configure(EntityTypeBuilder<Institution> builder)
        {
            builder.ToTable("Institutions");
            builder.HasKey(e => e.Id);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.OwnsOne(i => i.PriorityLevel, pl =>
            {
                pl.Property(p => p.Level).HasColumnName("PriorityLevel");
            });

            builder.HasOne(i => i.Parent)
                .WithMany(i => i.Children)
                .HasForeignKey(i => i.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Branches)
                .WithOne(b => b.Institution)
                .HasForeignKey(b => b.InstitutionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.Children)
                .WithOne(i => i.Parent)
                .HasForeignKey(i => i.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Employees)
                .WithOne(u => u.Institution)
                .HasForeignKey(u => u.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}