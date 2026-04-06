using BookingSystem.Core.Entities.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class MeetingInvitationConfiguration : IEntityTypeConfiguration<MeetingInvitation>
{
    public void Configure(EntityTypeBuilder<MeetingInvitation> builder)
    {
        builder.ToTable("MeetingInvitations");

        builder.HasKey(mi => mi.Id);

        builder.HasOne(mi => mi.Meeting)
            .WithMany(m => m.Invitations)
            .HasForeignKey(mi => mi.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mi => mi.Invitee)
            .WithMany()
            .HasForeignKey(mi => mi.InviteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mi => mi.Inviter)
            .WithMany()
            .HasForeignKey(mi => mi.InviterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(mi => mi.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(mi => mi.CreatedAt)
            .IsRequired();

        builder.Property(mi => mi.RespondedAt)
            .IsRequired(false);
    }
}
