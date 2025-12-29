using FluentAssertions;
using HM.Application.Bookings.AddBooking;
using HM.Application.Bookings.CheckInGuest;
using HM.Application.Rooms.AddRoom;
using HM.Application.Users.AddUser;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Bookings;

public class CheckInGuestTests : BaseIntegrationTest
{
    public CheckInGuestTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_CheckIn_WhenBookingExistsAndStatusIsConfirmed()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(4, 401),
            new List<Feature>(),
            new Money(100, Currency.Usd));
        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.FirstAsync();

        var addUserCommand = new AddUserCommand(
            "Bob",
            "Builder",
            "555555555",
            "+1",
            "bob@test.com",
            new DateOnly(1980, 1, 1));
        var userResult = await Sender.Send(addUserCommand);

        var addBookingCommand = new AddBookingCommand(
            userResult.Value,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            room.Id);
        var bookingResult = await Sender.Send(addBookingCommand);

        // Act
        var checkInCommand = new CheckInGuestCommand(bookingResult.Value);
        var result = await Sender.Send(checkInCommand);

        // Assert
        result.ShouldBeSuccess();

        var booking = await DbContext.Bookings.FindAsync(bookingResult.Value);
        booking.Should().NotBeNull();
        booking.Status.Should().Be(BookingStatus.CheckedIn);
    }
}