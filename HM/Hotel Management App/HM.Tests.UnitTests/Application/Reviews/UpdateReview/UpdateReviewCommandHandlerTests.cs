using FluentAssertions;
using HM.Application.Reviews.UpdateReview;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using HM.Domain.Reviews.Value_Objects;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Reviews.UpdateReview;

public class UpdateReviewCommandHandlerTests
{
    private readonly UpdateReviewCommandHandler _handler;
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateReviewCommandHandlerTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _timeMock = new Mock<ITime>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UpdateReviewCommandHandler(
            _reviewRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _timeMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ReviewExists()
    {
        // Arrange
        var command = new UpdateReviewCommand(
            Guid.NewGuid(),
            4,
            new Comment("Updated Title", "Updated Content"));

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        var review = RoomReview.Create(Guid.NewGuid(), Guid.NewGuid(), new Comment("Old", "Old"), 3,
            DateTime.UtcNow.AddDays(-1));

        _reviewRepositoryMock
            .Setup(x => x.GetReview(command.ReviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(review));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        review.Rating.Should().Be(4);
        review.Comment.Title.Should().Be("Updated Title");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var command = new UpdateReviewCommand(
            Guid.NewGuid(),
            4,
            new Comment("Updated Title", "Updated Content"));

        _reviewRepositoryMock
            .Setup(x => x.GetReview(command.ReviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<RoomReview>(new Error("Review.NotFound", "Not found")));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Review.NotFound");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}