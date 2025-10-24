using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Domain.Reviews.Entities;

public sealed class RoomReview : Entity
{
    public RoomReview(Guid id, Guid roomId, Guid userId, Comment comment, int rating, DateTime createdAtUtc) : base(id)
    {
        RoomId = roomId;
        UserId = userId;
        Comment = comment;
        Rating = rating;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid RoomId { get; }
    public Guid UserId { get; }
    public Comment Comment { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(int newRating, Comment newComment, DateTime nowUtc)
    {
        var oldRating = Rating;
        Rating = newRating;
        UpdatedAtUtc = nowUtc;
        Comment = newComment;
    }
}
