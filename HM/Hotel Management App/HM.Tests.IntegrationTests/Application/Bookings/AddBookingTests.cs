using FluentAssertions;
using HM.Application.Bookings.AddBooking;
using HM.Application.Rooms.AddRoom;
using HM.Application.Users.AddUser;
using HM.Domain.Bookings;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Bookings;

public class AddBookingTests : BaseIntegrationTest
{
    public AddBookingTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddBooking_WhenUserAndRoomExist()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Double,
            new RoomLocation(3, 305),
            new List<Feature> { Feature.WiFi },
            new Money(200, Currency.Usd));
        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.FirstAsync();

        var addUserCommand = new AddUserCommand(
            "Alice",
            "Wonderland",
            "123123123",
            "+1",
            "alice@test.com",
            new DateOnly(2000, 1, 1));
        var userResult = await Sender.Send(addUserCommand);
        var userId = userResult.Value;

        // Act
        var bookingCommand = new AddBookingCommand(
            userId,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            room.Id);

        var result = await Sender.Send(bookingCommand);

        // Assert
        result.ShouldBeSuccess();

        var booking = await DbContext.Bookings.FindAsync(result.Value);
        booking.Should().NotBeNull();
        booking.RoomId.Should().Be(room.Id);
        booking.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_BookingOverlaps()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Double,
            new RoomLocation(3, 306),
            new List<Feature> { Feature.WiFi },
            new Money(200, Currency.Usd));
        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.OrderByDescending(x => x.Id).FirstAsync();

        var addUserCommand = new AddUserCommand(
            "Bob",
            "Builder",
            "321321321",
            "+1",
            "bob@test.com",
            new DateOnly(1980, 1, 1));
        var userResult = await Sender.Send(addUserCommand);
        var userId = userResult.Value;

        // Existing booking
        var bookingCommand1 = new AddBookingCommand(
            userId,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            room.Id);
        await Sender.Send(bookingCommand1);

        // Overlapping booking
        var bookingCommand2 = new AddBookingCommand(
            userId,
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            room.Id);

        // Act
        var result = await Sender.Send(bookingCommand2);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.Overlapping);
    }
}