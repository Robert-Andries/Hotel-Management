using FluentAssertions;
using HM.Application.Bookings.CreateBookingForGuest;
using HM.Application.Rooms.AddRoom;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Bookings;

public class CreateBookingTests : BaseIntegrationTest
{
    public CreateBookingTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateBooking_WhenRoomIsAvailable()
    {
        var addRoomCommand = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(1, 101),
            new List<Feature>(),
            new Money(100, Currency.Usd));

        var roomResult = await Sender.Send(addRoomCommand);
        roomResult.ShouldBeSuccess();

        var room = await DbContext.Rooms.FirstOrDefaultAsync();
        room.Should().NotBeNull();

        // Create a booking for a new guest
        var bookingCommand = new CreateBookingForGuestCommand(
            "Jane",
            "Doe",
            "jane.doe@test.com",
            "987654321",
            "+1",
            new DateOnly(1995, 5, 20),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            room.Id);
        var bookingResult = await Sender.Send(bookingCommand);

        bookingResult.ShouldBeSuccess();

        // Verify Booking
        var booking = await DbContext.Bookings.FindAsync(bookingResult.Value);
        booking.Should().NotBeNull();
        booking.UserId.Should().NotBeEmpty();
        booking.RoomId.Should().Be(room.Id);

        // Verify User was created correctly
        var user = await DbContext.Users.FindAsync(booking.UserId);
        user.Should().NotBeNull();
        user.Contact.Email.Domain.Should().Be("test.com");
        user.Contact.Email.Value.Should().Be("jane.doe");
        user.Contact.PhoneNumber.CountryCode.Should().Be("+1");
        user.Contact.PhoneNumber.Value.Should().Be("987654321");
        user.Name.FirstName.Should().Be("Jane");
        user.Name.LastName.Should().Be("Doe");
        user.DateOfBirth.Should().Be(new DateOnly(1995, 5, 20));
    }
}