using FluentAssertions;
using HM.Application.Bookings.CreateBookingForGuest;
using HM.Application.Users.Services;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
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

namespace HM.Tests.UnitTests.Application.Bookings.CreateBookingForGuest;

public class CreateBookingForGuestCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly CreateBookingForGuestCommandHandler _handler;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public CreateBookingForGuestCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _pricingServiceMock = new Mock<IPricingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _timeMock = new Mock<ITime>();

        var userCreationService = new UserCreationService(_timeMock.Object, _userRepositoryMock.Object);

        _handler = new CreateBookingForGuestCommandHandler(
            _userRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            _pricingServiceMock.Object,
            _unitOfWorkMock.Object,
            _timeMock.Object,
            userCreationService);
    }

    [Fact]
    public async Task Handle_Should_CreateUserAndBooking_When_GuestIsNew()
    {
        // Arrange
        var command = CreateCommand();
        var room = CreateRoom();
        var totalPrice = new Money(200, Currency.Usd);

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        // User not found
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>(UserErrors.NotFound));

        // Room found
        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(room));

        // No overlap -- IsOverlappingAsync returns false
        _bookingRepositoryMock
            .Setup(x => x.IsOverlappingAsync(room,
                It.IsAny<DateRange>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        // Price calculation
        _pricingServiceMock
            .Setup(x => x.CalculatePrice(room, It.IsAny<DateRange>()))
            .Returns(new PricingDetails(totalPrice, Money.Zero(Currency.Usd), totalPrice));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        // Verify User Added
        _userRepositoryMock.Verify(x =>
            x.Add(It.Is<User>(u => u.Contact.Email.ToString() == command.Email)), Times.Once);

        // Verify Booking Added
        _bookingRepositoryMock.Verify(x =>
            x.AddAsync(It.Is<Booking>(b => b.Price == totalPrice),
                It.IsAny<CancellationToken>()), Times.Once);

        // Verify UnitOfWork
        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ExistingUserAndRoomAvailable()
    {
        // Arrange
        var command = CreateCommand();
        var room = CreateRoom();
        var totalPrice = new Money(200, Currency.Usd);
        var existingUser = User.Create(
            new Name("Existing", "User"),
            new ContactInfo(Email.Create(command.Email).Value,
                PhoneNumber.Create(command.PhoneNumber, command.CountryCode).Value),
            command.DateOfBirth,
            DateOnly.FromDateTime(DateTime.UtcNow)).Value;

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        // User found
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(room));

        _bookingRepositoryMock
            .Setup(x => x.IsOverlappingAsync(room, It.IsAny<DateRange>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _pricingServiceMock
            .Setup(x => x.CalculatePrice(room, It.IsAny<DateRange>()))
            .Returns(new PricingDetails(totalPrice, Money.Zero(Currency.Usd), totalPrice));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify User was NOT added again
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);

        // Verify Booking Added with existing user ID
        _bookingRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Booking>(b => b.UserId == existingUser.Id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoomOverlaps()
    {
        // Arrange
        var command = CreateCommand();
        var room = CreateRoom();

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        // User not found (irrelevant to overlap, but needed for flow)
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>(UserErrors.NotFound));

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(room));

        // Overlap exists
        _bookingRepositoryMock
            .Setup(x => x.IsOverlappingAsync(room, It.IsAny<DateRange>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.Overlapping);

        // Verify Booking NOT Added
        _bookingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CreateBookingForGuestCommand CreateCommand()
    {
        return new CreateBookingForGuestCommand(
            "John",
            "Doe",
            "john.doe@example.com",
            "123456789",
            "+1",
            new DateOnly(1990, 1, 1),
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 3),
            Guid.NewGuid());
    }

    private static Room CreateRoom()
    {
        return Room.Create(
            RoomType.Single,
            new RoomLocation(1, 101),
            new List<Feature>(),
            new Money(100, Currency.Usd)
        ).Value;
    }
}