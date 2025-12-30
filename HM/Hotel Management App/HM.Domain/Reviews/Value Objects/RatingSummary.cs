using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;

namespace HM.Domain.Reviews.Value_Objects;

/// <summary>
///     Aggregates rating statistics for a room.
/// </summary>
/// <param name="RoomId">The ID of the room.</param>
/// <param name="Average">The average rating score.</param>
/// <param name="Count">The total number of reviews.</param>
public sealed record RatingSummary(Guid RoomId, float Average, int Count)
{
    private RatingSummary() : this(Guid.Empty, 0, 0)
    {
    }

    /// <summary>
    ///     Recalculates the rating summary by fetching latest reviews from the repository.
    /// </summary>
    /// <param name="roomRepository">The review repository.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new RatingSummary with updated values.</returns>
    public async Task<Result<RatingSummary>> Update(IReviewRepository roomRepository,
        CancellationToken cancellationToken = default)
    {
        var reviewsResult = await roomRepository.GetRoomReviews(RoomId, cancellationToken);
        if (reviewsResult.IsFailure)
            return Result.Failure<RatingSummary>(reviewsResult.Error);

        var reviews = reviewsResult.Value;

        if (reviews.Count == 0) return Result.Success(new RatingSummary(RoomId, 0, 0));

        var newAvg = (float)reviews.Average(x => x.Rating);
        var newCount = reviews.Count;

        return Result.Success(new RatingSummary(RoomId, newAvg, newCount));
    }

    public override string ToString()
    {
        return $"{Average:F2} ({Count})";
    }
}