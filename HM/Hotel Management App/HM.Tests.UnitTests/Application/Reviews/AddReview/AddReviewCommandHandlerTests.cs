using FluentAssertions;
using HM.Application.Reviews.AddReview;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Reviews.AddReview;

public class AddReviewCommandHandlerTests
{
    private readonly AddReviewCommandHandler _handler;
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AddReviewCommandHandlerTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _timeMock = new Mock<ITime>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AddReviewCommandHandler(
            _timeMock.Object,
            _reviewRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RoomExists()
    {
        // Arrange
        var command = new AddReviewCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new Comment("Title", "Content"),
            5);

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feautre>(),
            new Money(10, Currency.Usd)).Value;

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(room));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _reviewRepositoryMock.Verify(x => x.AddReview(It.IsAny<RoomReview>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoomDoesNotExist()
    {
        // Arrange
        var command = new AddReviewCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new Comment("Title", "Content"),
            5);

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Room>(new Error("Room.NotFound", "Not found")));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Room.NotFound");

        _reviewRepositoryMock.Verify(x => x.AddReview(It.IsAny<RoomReview>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}