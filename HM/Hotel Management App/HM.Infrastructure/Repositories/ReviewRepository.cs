using HM.Domain.Abstractions;
using HM.Domain.Reviews;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using Microsoft.EntityFrameworkCore;

namespace HM.Infrastructure.Repositories;

/// <summary>
///     Repository for managing review entities.
/// </summary>
public sealed class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReviewRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddReview(RoomReview review, CancellationToken cancellationToken = default)
    {
        _dbContext.Reviews.Add(review);
    }

    public async Task<Result<List<RoomReview>>> GetRoomReviews(Guid roomId,
        CancellationToken cancellationToken = default)
    {
        var reviews = await _dbContext.Reviews
            .Where(r => r.RoomId == roomId)
            .ToListAsync(cancellationToken);

        return Result.Success(reviews);
    }

    public async Task<Result<RoomReview>> GetReview(Guid reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
        if (review is null) return Result.Failure<RoomReview>(ReviewErrors.NotFound);

        return Result.Success(review);
    }

    public async Task<Result<List<RoomReview>>> GetUserReviews(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var reviews = await _dbContext.Reviews
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);

        return Result.Success(reviews);
    }
}