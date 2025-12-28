using FluentAssertions;
using HM.Application.Abstractions.Data;
using HM.Application.Rooms.GetAllRooms;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Services;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.UnitTests.Helpers;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Rooms.GetAllRooms;

public class GetAllRoomsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetAllRoomsQueryHandler _handler;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly Mock<ITime> _timeMock;

    public GetAllRoomsQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _pricingServiceMock = new Mock<IPricingService>();
        _timeMock = new Mock<ITime>();
        _handler = new GetAllRoomsQueryHandler(_contextMock.Object, _pricingServiceMock.Object, _timeMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAllRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feautre>(), new Money(100, Currency.Usd))
                .Value,
            Room.Create(RoomType.Double, new RoomLocation(1, 102), new List<Feautre>(), new Money(150, Currency.Usd))
                .Value
        };

        _contextMock.Setup(x => x.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(rooms));

        var query = new GetAllRoomsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}