using FluentAssertions;
using HM.Application.Rooms.FinishMaintenance;
using HM.Domain.Abstractions;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Rooms.FinishMaintenance;

public class FinishMaintenanceCommandHandlerTests
{
    private readonly FinishMaintenanceCommandHandler _handler;
    private readonly Mock<ILogger<FinishMaintenanceCommandHandler>> _loggerMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;

    public FinishMaintenanceCommandHandlerTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _loggerMock = new Mock<ILogger<FinishMaintenanceCommandHandler>>();
        _handler = new FinishMaintenanceCommandHandler(_loggerMock.Object, _roomRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RoomIsUnderMaintenance()
    {
        // Arrange
        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(100, Currency.Usd)).Value;

        // Move to Occupied then Maintenance
        room.Reserve(DateTime.UtcNow);
        room.Occupy();
        room.ReleaseForMaintenance(); // Now in Maintenance status

        _roomRepositoryMock.Setup(x => x.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(room));

        _roomRepositoryMock.Setup(x => x.UpdateRoomAsync(room.Id, room, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var command = new FinishMaintenanceCommand(room.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Status.Should().Be(RoomStatus.Available);
        _roomRepositoryMock.Verify(x => x.UpdateRoomAsync(room.Id, room, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoomNotInMaintenance()
    {
        // Arrange
        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(100, Currency.Usd)).Value;
        // Status is Available

        _roomRepositoryMock.Setup(x => x.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(room));

        var command = new FinishMaintenanceCommand(room.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.InvalidStatus);

        _roomRepositoryMock.Verify(
            x => x.UpdateRoomAsync(It.IsAny<Guid>(), It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}