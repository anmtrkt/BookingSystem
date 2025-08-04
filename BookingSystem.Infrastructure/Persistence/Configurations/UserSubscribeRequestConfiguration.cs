using BookingSystem.Core.Domain.Entities;
using BookingSystem.Core.Domain.Entities.Institutions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Persistence.Configurations
{
    public class UserSubscribeRequestConfiguration : IEntityTypeConfiguration<UserSubscribeRequest>
    {
        public void Configure(EntityTypeBuilder<UserSubscribeRequest> builder)
        {
            builder.ToTable("UserSubscribeRequest");
            builder.HasNoKey();

            builder.Property(b => b.UserId)
                .IsRequired()
                .HasMaxLength(512);
            builder.Property(b => b.MeetingId)
          .IsRequired()
          .HasMaxLength(512);
        }
    }
}
