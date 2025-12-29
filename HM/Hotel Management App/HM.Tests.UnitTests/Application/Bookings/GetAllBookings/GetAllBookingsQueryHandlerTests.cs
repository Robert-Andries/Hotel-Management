using FluentAssertions;
using HM.Application.Abstractions.Data;
using HM.Application.Bookings.GetAllBookings;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;
using HM.Tests.UnitTests.Helpers;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Bookings.GetAllBookings;

public class GetAllBookingsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetAllBookingsQueryHandler _handler;

    public GetAllBookingsQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetAllBookingsQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnActiveBookings_When_SeeCompletedIsFalse()
    {
        // Arrange
        // User
        var user = User.Create(
            new Name("John", "Doe"),
            new ContactInfo(Email.Create("test@test.com").Value, PhoneNumber.Create("123456789", "+1").Value),
            new DateOnly(1990, 1, 1),
            DateOnly.FromDateTime(DateTime.UtcNow)).Value;

        // Room
        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(100, Currency.Usd)).Value;

        // Bookings
        var activeBooking = Booking.Reserve(room.Id, user.Id,
            DateRange.Create(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 5)).Value, DateTime.UtcNow,
            new Money(400, Currency.Usd));

        var completedBooking = Booking.Reserve(room.Id, user.Id,
            DateRange.Create(new DateOnly(2022, 1, 1), new DateOnly(2022, 1, 5)).Value, DateTime.UtcNow.AddYears(-1),
            new Money(400, Currency.Usd));
        completedBooking.CheckIn(DateTime.UtcNow.AddYears(-1));
        completedBooking.CheckOut(DateTime.UtcNow.AddYears(-1)); // Status Completed

        var bookings = new List<Booking> { activeBooking, completedBooking };
        var users = new List<User> { user };
        var rooms = new List<Room> { room };

        _contextMock.Setup(x => x.Users).Returns(MockDbSetHelper.GetQueryableMockDbSet(users));
        _contextMock.Setup(x => x.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(rooms));
        _contextMock.Setup(x => x.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(bookings));

        var query = new GetAllBookingsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Id.Should().Be(activeBooking.Id);
    }

    [Fact]
    public async Task Handle_Should_ReturnAllBookings_When_SeeCompletedIsTrue()
    {
        // Arrange
        var user = User.Create(
            new Name("John", "Doe"),
            new ContactInfo(Email.Create("test@test.com").Value, PhoneNumber.Create("123456789", "+1").Value),
            new DateOnly(1990, 1, 1),
            DateOnly.FromDateTime(DateTime.UtcNow)).Value;
        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(100, Currency.Usd)).Value;

        var activeBooking = Booking.Reserve(room.Id, user.Id,
            DateRange.Create(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 5)).Value, DateTime.UtcNow,
            new Money(400, Currency.Usd));
        var completedBooking = Booking.Reserve(room.Id, user.Id,
            DateRange.Create(new DateOnly(2022, 1, 1), new DateOnly(2022, 1, 5)).Value, DateTime.UtcNow.AddYears(-1),
            new Money(400, Currency.Usd));
        completedBooking.CheckIn(DateTime.UtcNow.AddYears(-1));
        completedBooking.CheckOut(DateTime.UtcNow.AddYears(-1));

        var bookings = new List<Booking> { activeBooking, completedBooking };
        var users = new List<User> { user };
        var rooms = new List<Room> { room };

        _contextMock.Setup(x => x.Users).Returns(MockDbSetHelper.GetQueryableMockDbSet(users));
        _contextMock.Setup(x => x.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(rooms));
        _contextMock.Setup(x => x.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(bookings));

        var query = new GetAllBookingsQuery(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}