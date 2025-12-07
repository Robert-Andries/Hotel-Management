using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HM.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.OwnsOne(b => b.Duration);

        builder.OwnsOne(b => b.Price, price =>
        {
            price.Property(p => p.Currency)
                .HasConversion(c => c.Code, code => Currency.FromCode(code))
                .HasMaxLength(3);
        });

        builder.Property(b => b.Status);
        
        builder.Property(b => b.ReservedOnUtc);
        builder.Property(b => b.CheckedInOnUtc);
        builder.Property(b => b.CancelledOnUtc);
        builder.Property(b => b.CompletedOnUtc);
        
        builder.HasOne<HM.Domain.Users.Entities.User>()
            .WithMany()
            .HasForeignKey(b => b.UserId);

        builder.HasOne<HM.Domain.Rooms.Entities.Room>()
            .WithMany()
            .HasForeignKey(b => b.RoomId);
    }
}
