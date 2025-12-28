using FluentAssertions;
using HM.Application.Bookings.CheckInGuest;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Shared;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Bookings.CheckInGuest;

public class CheckInGuestCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly CheckInGuestCommandHandler _handler;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CheckInGuestCommandHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _timeMock = new Mock<ITime>();
        _handler = new CheckInGuestCommandHandler(_unitOfWorkMock.Object, _bookingRepositoryMock.Object,
            _timeMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BookingIsReserved()
    {
        // Arrange
        var command = new CheckInGuestCommand(Guid.NewGuid());
        var now = DateTime.UtcNow;
        _timeMock.Setup(x => x.NowUtc).Returns(now);

        var booking = Booking.Reserve(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateRange.Create(DateOnly.FromDateTime(now), DateOnly.FromDateTime(now.AddDays(1))).Value,
            now.AddDays(-1),
            new Money(100, Currency.Usd)); // Status is Reserved

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(booking));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.CheckedIn);
        booking.CheckedInOnUtc.Should().Be(now);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BookingNotFound()
    {
        // Arrange
        var command = new CheckInGuestCommand(Guid.NewGuid());
        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Booking>(new Error("Booking.NotFound", "Not found")));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BookingStatusIsInvalid()
    {
        // Arrange
        var command = new CheckInGuestCommand(Guid.NewGuid());
        var booking = Booking.Reserve(Guid.NewGuid(), Guid.NewGuid(),
            DateRange.Create(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 2)).Value, DateTime.UtcNow,
            new Money(100, Currency.Usd));
        booking.CheckIn(DateTime.UtcNow); //Status CheckedIn

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(booking));

        // Try to CheckIn again (Should fail)

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotReserved);
    }
}