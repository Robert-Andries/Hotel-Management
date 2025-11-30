using HM.Domain.Abstractions;
using HM.Domain.Reviews.Events;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Domain.Reviews.Entities;

public sealed class RoomReview : Entity
{
    private RoomReview(Guid id, Guid roomId, Guid userId, Comment comment, int rating, DateTime createdAtUtc) : base(id)
    {
        RoomId = roomId;
        UserId = userId;
        Comment = comment;
        Rating = rating;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid RoomId { get; init; }
    public Guid UserId { get; init; }
    public Comment Comment { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; internal set; }

    public static RoomReview Create(Guid roomId, Guid userId, Comment comment, int rating, DateTime createdAtUtc)
    {
        var review = new RoomReview(Guid.NewGuid(),
            roomId,
            userId,
            comment,
            rating,
            createdAtUtc);

        review.RaiseDomainEvent(new ReviewCreatedDomainEvent(review.Id, review.RoomId));

        return review;
    }
    public void Update(int rating, Comment comment, DateTime updatedAtUtc)
    {
        Rating = rating;
        Comment = comment;
        UpdatedAtUtc = updatedAtUtc;

        this.RaiseDomainEvent(new ReviewUpdatedDomainEvent(Id, RoomId));
    }
}