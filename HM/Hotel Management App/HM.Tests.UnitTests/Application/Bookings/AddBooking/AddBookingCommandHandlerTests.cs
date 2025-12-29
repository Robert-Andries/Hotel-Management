using FluentAssertions;
using HM.Application.Bookings.AddBooking;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Domain.Users;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Bookings.AddBooking;

public class AddBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly AddBookingCommandHandler _handler;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public AddBookingCommandHandlerTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _timeMock = new Mock<ITime>();
        _pricingServiceMock = new Mock<IPricingService>();

        _handler = new AddBookingCommandHandler(
            _roomRepositoryMock.Object,
            _userRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _timeMock.Object,
            _pricingServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Valid()
    {
        // Arrange
        var command = new AddBookingCommand(
            Guid.NewGuid(),
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 5),
            Guid.NewGuid());

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        // User Mock
        var user = User.Create(new Name("John", "Doe"),
            new ContactInfo(Email.Create("t@t.com").Value, PhoneNumber.Create("123", "+1").Value),
            new DateOnly(1990, 1, 1),
            DateOnly.FromDateTime(DateTime.UtcNow)).Value;

        _userRepositoryMock.Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Room Mock
        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(100, Currency.Usd)).Value;
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(room));

        // Booking Overlap Mock (No Overlap)
        _bookingRepositoryMock.Setup(x =>
                x.IsOverlappingAsync(It.IsAny<Room>(), It.IsAny<DateRange>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        // Pricing Mock
        _pricingServiceMock.Setup(x => x.CalculatePrice(It.IsAny<Room>(), It.IsAny<DateRange>()))
            .Returns(new PricingDetails(new Money(400, Currency.Usd), Money.Zero(Currency.Usd),
                new Money(400, Currency.Usd)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _bookingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var command = new AddBookingCommand(Guid.NewGuid(), new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 5),
            Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>(UserErrors.NotFound));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(UserErrors.NotFound.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoomNotFound()
    {
        // Arrange
        var command = new AddBookingCommand(Guid.NewGuid(), new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 5),
            Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(User.Create(new Name("f", "l"),
                new ContactInfo(Email.Create("a@a.com").Value, PhoneNumber.Create("123", "+1").Value),
                new DateOnly(1990, 1, 1), DateOnly.FromDateTime(DateTime.UtcNow)).Value));

        _roomRepositoryMock.Setup(x => x.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Room>(RoomErrors.NotFound));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(RoomErrors.NotFound.Code);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Overlapping()
    {
        // Arrange
        var command = new AddBookingCommand(Guid.NewGuid(), new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 5),
            Guid.NewGuid());

        // Mocks for successful user/room fetch
        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(User.Create(new Name("f", "l"),
                new ContactInfo(Email.Create("a@a.com").Value, PhoneNumber.Create("123", "+1").Value),
                new DateOnly(1990, 1, 1), DateOnly.FromDateTime(DateTime.UtcNow)).Value));
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(Room.Create(RoomType.Single, new RoomLocation(1, 1), new List<Feature>(),
                new Money(100, Currency.Usd)).Value));

        // Overlap Mock (Returns true = Yes Overlap)
        _bookingRepositoryMock.Setup(x =>
                x.IsOverlappingAsync(It.IsAny<Room>(), It.IsAny<DateRange>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.Overlapping);
    }
}