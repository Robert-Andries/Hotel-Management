using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;

namespace HM.Domain.Reviews.Value_Objects;

public sealed record RatingSummary(Guid RoomId, double Average, int Count)
{
    public async Task<Result<RatingSummary>> Update(IReviewRepository roomRepository,
        CancellationToken cancellationToken = default)
    {
        var reviewsResult = await roomRepository.GetRoomReviews(RoomId, cancellationToken);
        if (reviewsResult.IsFailure)
            return Result.Failure<RatingSummary>(reviewsResult.Error);

        var reviews = reviewsResult.Value;
        var newAvg = reviews.Average(x => x.Rating);
        var newCount = reviews.Count;

        return Result.Success(new RatingSummary(RoomId, newAvg, newCount));
    }
}