using FluentAssertions;
using HM.Application.Bookings.CheckOutGuest;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Shared;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Bookings.CheckOutGuest;

public class CheckOutGuestCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly CheckOutGuestCommandHandler _handler;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CheckOutGuestCommandHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _timeMock = new Mock<ITime>();
        _handler = new CheckOutGuestCommandHandler(_bookingRepositoryMock.Object, _unitOfWorkMock.Object,
            _timeMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BookingIsCheckedIn()
    {
        // Arrange
        var command = new CheckOutGuestCommand(Guid.NewGuid());
        var now = DateTime.UtcNow;
        _timeMock.Setup(x => x.NowUtc).Returns(now);

        var booking = Booking.Reserve(Guid.NewGuid(), Guid.NewGuid(),
            DateRange.Create(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 2)).Value, DateTime.UtcNow,
            new Money(100, Currency.Usd));
        booking.CheckIn(now.AddDays(-1)); // Status CheckedIn

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(booking));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Completed);
        booking.CompletedOnUtc.Should().Be(now);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BookingIsNotCheckedIn()
    {
        // Arrange
        var command = new CheckOutGuestCommand(Guid.NewGuid());
        var booking = Booking.Reserve(Guid.NewGuid(), Guid.NewGuid(),
            DateRange.Create(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 2)).Value, DateTime.UtcNow,
            new Money(100, Currency.Usd));
        // Status Reserved (Not CheckedIn)

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(booking));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotCheckedIn);
    }
}