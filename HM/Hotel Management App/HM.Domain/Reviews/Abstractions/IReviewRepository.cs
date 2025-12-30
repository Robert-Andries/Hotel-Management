using HM.Domain.Abstractions;
using HM.Domain.Reviews.Entities;

namespace HM.Domain.Reviews.Abstractions;

/// <summary>
///     Defines the contract for Review persistence operations.
/// </summary>
public interface IReviewRepository
{
    /// <summary>Retrieves all reviews for a specific room.</summary>
    Task<Result<List<RoomReview>>> GetRoomReviews(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single review by ID.</summary>
    Task<Result<RoomReview>> GetReview(Guid reviewId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all reviews written by a specific user.</summary>
    Task<Result<List<RoomReview>>> GetUserReviews(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new review to the repository.</summary>
    void AddReview(RoomReview review, CancellationToken cancellationToken = default);
}