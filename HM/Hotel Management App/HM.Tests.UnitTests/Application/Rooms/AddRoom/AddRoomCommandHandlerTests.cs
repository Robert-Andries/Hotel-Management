using FluentAssertions;
using HM.Application.Rooms.AddRoom;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Rooms.AddRoom;

public class AddRoomCommandHandlerTests
{
    private static readonly Error RepoError = new("Repo.Error", "Db Error");
    private readonly AddRoomCommandHandler _handler;
    private readonly Mock<ILogger<AddRoomCommandHandler>> _loggerMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;

    public AddRoomCommandHandlerTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _loggerMock = new Mock<ILogger<AddRoomCommandHandler>>();
        _handler = new AddRoomCommandHandler(_roomRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CommandIsValid()
    {
        // Arrange
        var command = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(1, 101),
            new List<Feautre>(),
            new Money(100, Currency.Usd));

        _roomRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _roomRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Room>(r => r.Location.RoomNumber == 101), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoomCreationFails()
    {
        // Arrange
        // Invalid Price to cause Room.Create failure
        var command = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(1, 101),
            new List<Feautre>(),
            new Money(0, Currency.Usd));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        // RoomErrors.InvalidPrice might be the error
        _roomRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RepositoryFails()
    {
        // Arrange
        var command = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(1, 101),
            new List<Feautre>(),
            new Money(100, Currency.Usd));

        _roomRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(RepoError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(RepoError.Code);
    }
}