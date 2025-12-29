using FluentAssertions;
using HM.Application.Abstractions.Data;
using HM.Application.Rooms.GetAvailableRooms;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.UnitTests.Helpers;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Rooms.GetAvailableRooms;

public class GetAvailableRoomsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetAvailableRoomsQueryHandler _handler;
    private readonly Mock<IPricingService> _pricingServiceMock;

    public GetAvailableRoomsQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _pricingServiceMock = new Mock<IPricingService>();
        _handler = new GetAvailableRoomsQueryHandler(_contextMock.Object, _pricingServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAvailableRooms_When_NoOverlap()
    {
        // Arrange
        // Date Range: 2023-01-01 to 2023-01-10
        var startDate = new DateOnly(2023, 1, 1);
        var endDate = new DateOnly(2023, 1, 10);
        var query = new GetAvailableRoomsQuery(startDate, endDate, new List<Feature>());

        // Room 1: Available
        var room1 = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(100, Currency.Usd)).Value;

        // Rooms Mock
        var rooms = new List<Room> { room1 };
        _contextMock.Setup(x => x.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(rooms));

        // Bookings Mock (Empty)
        var bookings = new List<Booking>();
        _contextMock.Setup(x => x.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(bookings));

        // Pricing Service Mock
        var price = new Money(900, Currency.Usd);
        _pricingServiceMock
            .Setup(x => x.CalculatePrice(It.IsAny<Room>(), It.IsAny<DateRange>()))
            .Returns(new PricingDetails(price, Money.Zero(Currency.Usd), price));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value.First().RoomId.Should().Be(room1.Id);
    }

    [Fact]
    public async Task Handle_Should_FilterOut_OverlappingRooms()
    {
        // Arrange
        var startDate = new DateOnly(2023, 1, 1);
        var endDate = new DateOnly(2023, 1, 10);
        var query = new GetAvailableRoomsQuery(startDate, endDate, new List<Feature>());

        var room1 = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(100, Currency.Usd)).Value;

        // Existing Booking for Room1: 2023-01-05 to 2023-01-07 (Overlaps)
        var booking = Booking.Reserve(
            room1.Id,
            Guid.NewGuid(),
            DateRange.Create(new DateOnly(2023, 1, 5), new DateOnly(2023, 1, 7)).Value,
            DateTime.UtcNow,
            new Money(100, Currency.Usd));

        var rooms = new List<Room> { room1 };
        var bookings = new List<Booking> { booking };

        _contextMock.Setup(x => x.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(rooms));
        _contextMock.Setup(x => x.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(bookings));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}