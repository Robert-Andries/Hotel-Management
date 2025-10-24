using HM.Domain.Abstractions;
using HM.Domain.Reviews.Entities;

namespace HM.Domain.Reviews.Abstractions;

public interface IReviewRepository
{
    Task<Result<List<RoomReview>>> GetRoomReviews(Guid roomId,CancellationToken cancellationToken = default);
    Task<Result<List<RoomReview>>> GetUserReviews(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> AddReview(RoomReview review, CancellationToken cancellationToken = default);
}