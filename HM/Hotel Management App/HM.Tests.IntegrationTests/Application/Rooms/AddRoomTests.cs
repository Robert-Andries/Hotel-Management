using FluentAssertions;
using HM.Application.Rooms.AddRoom;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Rooms;

public class AddRoomTests : BaseIntegrationTest
{
    public AddRoomTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddRoom_WhenCommandIsValid()
    {
        // Arrange
        var command = new AddRoomCommand(
            RoomType.Double,
            new RoomLocation(2, 202),
            new List<Feature> { Feature.WiFi, Feature.AirConditioning },
            new Money(150, Currency.Usd));

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.ShouldBeSuccess();

        var room = await DbContext.Rooms.FirstOrDefaultAsync();
        room.Should().NotBeNull();
        room!.RoomType.Should().Be(RoomType.Double);
        room.Location.Should().Be(new RoomLocation(2, 202));
        room.Price.Amount.Should().Be(150);
        room.Features.Should().Contain(Feature.WiFi);
        room.Features.Should().Contain(Feature.AirConditioning);
    }
}