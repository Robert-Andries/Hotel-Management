using FluentAssertions;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using HM.Domain.Reviews.Value_Objects;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Tests.IntegrationTests.Infrastructure.Repositories;

public class ReviewRepositoryTests : BaseIntegrationTest
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewRepositoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _reviewRepository = ServiceProvider.GetRequiredService<IReviewRepository>();
    }

    [Fact]
    public async Task AddReview_ShouldPersistReview_WhenReviewIsValid()
    {
        // Arrange
        var review = RoomReview.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new Comment("Repo", "Test"),
            5,
            DateTime.UtcNow);

        // Act
        _reviewRepository.AddReview(review);
        await DbContext.SaveChangesAsync();

        // Assert
        var fromDb = await DbContext.Reviews.FindAsync(review.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Id.Should().Be(review.Id);
    }

    [Fact]
    public async Task GetRoomReviews_ShouldReturnList_WhenReviewsExist()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var review1 = RoomReview.Create(roomId, Guid.NewGuid(), new Comment("R1", "C1"), 4, DateTime.UtcNow);
        var review2 = RoomReview.Create(roomId, Guid.NewGuid(), new Comment("R2", "C2"), 5, DateTime.UtcNow);

        _reviewRepository.AddReview(review1);
        _reviewRepository.AddReview(review2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _reviewRepository.GetRoomReviews(roomId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUserReviews_ShouldReturnList_WhenReviewsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var review1 = RoomReview.Create(Guid.NewGuid(), userId, new Comment("U1", "C1"), 3, DateTime.UtcNow);

        _reviewRepository.AddReview(review1);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _reviewRepository.GetUserReviews(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }
}