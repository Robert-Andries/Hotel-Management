using FluentAssertions;
using HM.Application.Rooms.AddRoom;
using HM.Application.Rooms.FinishMaintenance;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Rooms;

public class FinishMaintenanceTests : BaseIntegrationTest
{
    public FinishMaintenanceTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_FinishMaintenance_WhenRoomIsUnderMaintenance()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(1, 105),
            new List<Feature>(),
            new Money(50, Currency.Usd));

        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.FirstAsync();

        room.Occupy();
        room.ReleaseForMaintenance();
        await DbContext.SaveChangesAsync();

        // Act
        var finishCommand = new FinishMaintenanceCommand(room.Id);
        var result = await Sender.Send(finishCommand);

        // Assert
        result.ShouldBeSuccess();
        var updatedRoom = await DbContext.Rooms.FindAsync(room.Id);
        updatedRoom.Should().NotBeNull();
        updatedRoom.Status.Should().Be(RoomStatus.Available);
    }
}