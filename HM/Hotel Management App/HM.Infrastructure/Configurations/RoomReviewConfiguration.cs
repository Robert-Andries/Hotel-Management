using HM.Domain.Reviews.Entities;
using HM.Domain.Rooms.Entities;
using HM.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HM.Infrastructure.Configurations;

internal sealed class RoomReviewConfiguration : IEntityTypeConfiguration<RoomReview>
{
    public void Configure(EntityTypeBuilder<RoomReview> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(review => review.Id);

        builder.Property(review => review.Rating);

        builder.Property(review => review.CreatedAtUtc);

        builder.Property(review => review.UpdatedAtUtc);

        builder.OwnsOne(review => review.Comment, commentBuilder =>
        {
            commentBuilder.Property(comment => comment.Title)
                .HasMaxLength(200);

            commentBuilder.Property(comment => comment.Content)
                .HasMaxLength(4000);
        });

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(review => review.RoomId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(review => review.UserId);
    }
}