using HM.Domain.Abstractions;
using HM.Domain.Reviews.Events;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Domain.Reviews.Entities;

/// <summary>
///     Represents a guest's review of a room.
/// </summary>
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private RoomReview()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    /// <summary>Gets the ID of the room being reviewed.</summary>
    public Guid RoomId { get; init; }

    /// <summary>Gets the ID of the user who wrote the review.</summary>
    public Guid UserId { get; init; }

    /// <summary>Gets the review content.</summary>
    public Comment Comment { get; private set; }

    /// <summary>Gets the numerical rating provided by the user.</summary>
    public int Rating { get; private set; }

    /// <summary>Gets the timestamp when the review was created.</summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>Gets the timestamp when the review was last updated.</summary>
    public DateTime? UpdatedAtUtc { get; internal set; }

    /// <summary>
    ///     Creates a new RoomReview entity.
    /// </summary>
    /// <param name="roomId">The ID of the room.</param>
    /// <param name="userId">The ID of the author.</param>
    /// <param name="comment">The review content.</param>
    /// <param name="rating">The numeric rating.</param>
    /// <param name="createdAtUtc">Creation timestamp.</param>
    /// <returns>The newly created RoomReview.</returns>
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

    /// <summary>
    ///     Updates the review content and rating.
    /// </summary>
    /// <param name="rating">The new rating.</param>
    /// <param name="comment">The new comment content.</param>
    /// <param name="updatedAtUtc">Update timestamp.</param>
    public void Update(int rating, Comment comment, DateTime updatedAtUtc)
    {
        Rating = rating;
        Comment = comment;
        UpdatedAtUtc = updatedAtUtc;

        RaiseDomainEvent(new ReviewUpdatedDomainEvent(Id, RoomId));
    }
}