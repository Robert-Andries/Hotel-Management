using FluentAssertions;
using HM.Application.Abstractions.Data;
using HM.Application.Rooms.FindBestRoom;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.UnitTests.Helpers;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Rooms.FindBestRoom;

public class FindBestRoomQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly FindBestRoomQueryHandler _handler;
    private readonly Mock<IPricingService> _pricingServiceMock;

    public FindBestRoomQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _pricingServiceMock = new Mock<IPricingService>();
        _handler = new FindBestRoomQueryHandler(_contextMock.Object, _pricingServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoRoomsAvailable()
    {
        // Arrange
        var query = new FindBestRoomQuery(
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 10),
            RoomType.Single,
            new List<Feature>());

        _contextMock.Setup(x => x.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Room>()));
        _contextMock.Setup(x => x.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Booking>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.NoneAvailable);
    }

    [Fact]
    public async Task Handle_Should_ReturnBestMatchingRoom_When_Available()
    {
        // Arrange
        var query = new FindBestRoomQuery(
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 10),
            RoomType.Single,
            new List<Feature> { Feature.WiFi });

        // Room 1: Fits criteria, Expensive
        var room1 = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature> { Feature.WiFi },
            new Money(200, Currency.Usd)).Value;

        // Room 2: Fits criteria, Cheaper
        var room2 = Room.Create(RoomType.Single, new RoomLocation(1, 102), new List<Feature> { Feature.WiFi },
            new Money(100, Currency.Usd)).Value;

        // Room 3: Features mismatch
        var room3 = Room.Create(RoomType.Single, new RoomLocation(1, 103), new List<Feature>(),
            new Money(50, Currency.Usd)).Value;

        var rooms = new List<Room> { room1, room2, room3 };
        _contextMock.Setup(x => x.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(rooms));
        _contextMock.Setup(x => x.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Booking>()));

        var price = new Money(900, Currency.Usd);
        _pricingServiceMock
            .Setup(x => x.CalculatePrice(It.IsAny<Room>(), It.IsAny<DateRange>()))
            .Returns(new PricingDetails(price, Money.Zero(Currency.Usd), price));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RoomId.Should().Be(room2.Id); // Should be room2 (Cheap & Fits)
    }
}