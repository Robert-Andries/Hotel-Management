using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;

namespace HM.Domain.Reviews.Value_Objects;

public sealed record RatingSummary(Guid RoomId, float Average, int Count)
{
    private RatingSummary() : this(Guid.Empty, 0, 0)
    {
    }

    public async Task<Result<RatingSummary>> Update(IReviewRepository roomRepository,
        CancellationToken cancellationToken = default)
    {
        var reviewsResult = await roomRepository.GetRoomReviews(RoomId, cancellationToken);
        if (reviewsResult.IsFailure)
            return Result.Failure<RatingSummary>(reviewsResult.Error);

        var reviews = reviewsResult.Value;
        var newAvg = (float)reviews.Average(x => x.Rating);
        var newCount = reviews.Count;

        return Result.Success(new RatingSummary(RoomId, newAvg, newCount));
    }

    public override string ToString()
    {
        return $"{Average:F2} ({Count})";
    }
}