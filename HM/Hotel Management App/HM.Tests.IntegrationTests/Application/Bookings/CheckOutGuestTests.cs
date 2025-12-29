using FluentAssertions;
using HM.Application.Bookings.AddBooking;
using HM.Application.Bookings.CheckInGuest;
using HM.Application.Bookings.CheckOutGuest;
using HM.Application.Rooms.AddRoom;
using HM.Application.Users.AddUser;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Bookings;

public class CheckOutGuestTests : BaseIntegrationTest
{
    public CheckOutGuestTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_CheckOut_WhenBookingExistsAndStatusIsActive()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(5, 501),
            new List<Feature>(),
            new Money(100, Currency.Usd));
        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.FirstAsync();

        var addUserCommand = new AddUserCommand(
            "Charlie",
            "Chaplin",
            "999999999",
            "+44",
            "charlie@test.com",
            new DateOnly(1980, 4, 16));
        var userResult = await Sender.Send(addUserCommand);

        var addBookingCommand = new AddBookingCommand(
            userResult.Value,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            room.Id);
        var bookingResult = await Sender.Send(addBookingCommand);

        await Sender.Send(new CheckInGuestCommand(bookingResult.Value));

        // Act
        var checkOutCommand = new CheckOutGuestCommand(bookingResult.Value);
        var result = await Sender.Send(checkOutCommand);

        // Assert
        result.ShouldBeSuccess();

        var booking = await DbContext.Bookings.FindAsync(bookingResult.Value);
        booking.Should().NotBeNull();
        booking!.Status.Should().Be(BookingStatus.Completed);
    }
}