using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HM.Infrastructure.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.OwnsOne(r => r.Location);

        builder.OwnsOne(r => r.Price, price =>
        {
            price.Property(p => p.Currency)
                .HasConversion(c => c.Code, code => Currency.FromCode(code))
                .HasMaxLength(3);
        });

        builder.OwnsOne(r => r.Rating);

        builder.Property(r => r.Features)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => Enum.Parse<Feautre>(s))
                      .ToList());
    }
}
