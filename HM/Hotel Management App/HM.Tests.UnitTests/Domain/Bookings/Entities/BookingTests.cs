using FluentAssertions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Events;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Shared;
using Xunit;

namespace HM.Tests.UnitTests.Domain.Bookings.Entities;

public class BookingTests
{
    private static readonly Guid RoomId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    private static readonly DateRange Duration = DateRange.Create(
        DateOnly.FromDateTime(DateTime.UtcNow),
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))).Value;

    private static readonly Money Price = new(100, Currency.Usd);
    private static readonly DateTime UtcNow = DateTime.UtcNow;

    [Fact]
    public void Reserve_Should_SetStatusToReserved_When_BookingIsCreated()
    {
        // Act
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);

        // Assert
        booking.Status.Should().Be(BookingStatus.Reserved);
        booking.ReservedOnUtc.Should().Be(UtcNow);
        booking.UserId.Should().Be(UserId);
        booking.RoomId.Should().Be(RoomId);
        booking.Duration.Should().Be(Duration);
        booking.Price.Should().Be(Price);
    }

    [Fact]
    public void Reserve_Should_RaiseBookingReservedDomainEvent()
    {
        // Act
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);

        // Assert
        booking.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<BookingReservedDomainEvent>();
    }

    [Fact]
    public void CheckIn_Should_ReturnFailure_When_StatusIsNotReserved()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);
        booking.CheckIn(UtcNow); // Transitions to CheckedIn

        // Act
        var result = booking.CheckIn(UtcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotReserved);
    }

    [Fact]
    public void CheckIn_Should_SetStatusToCheckedIn_When_StatusIsReserved()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);

        // Act
        var result = booking.CheckIn(UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.CheckedIn);
        booking.CheckedInOnUtc.Should().Be(UtcNow);
    }

    [Fact]
    public void CheckIn_Should_RaiseBookingCheckedInDomainEvent()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);

        // Act
        booking.CheckIn(UtcNow);

        // Assert
        booking.GetDomainEvents().Should().ContainSingle(e => e is BookingCheckedInDomainEvent);
    }

    [Fact]
    public void CheckOut_Should_ReturnFailure_When_StatusIsNotCheckedIn()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);
        // Status is Reserved, not CheckedIn

        // Act
        var result = booking.CheckOut(UtcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotCheckedIn);
    }

    [Fact]
    public void CheckOut_Should_SetStatusToCompleted_When_StatusIsCheckedIn()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);
        booking.CheckIn(UtcNow);

        // Act
        var result = booking.CheckOut(UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Completed);
        booking.CompletedOnUtc.Should().Be(UtcNow);
    }

    [Fact]
    public void CheckOut_Should_RaiseBookingCheckedOutDomainEvent()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);
        booking.CheckIn(UtcNow);
        booking.ClearDomainEvents(); // Clear previous events

        // Act
        booking.CheckOut(UtcNow);

        // Assert
        booking.GetDomainEvents().Should().ContainSingle(e => e is BookingCheckedOutDomainEvent);
    }

    [Fact]
    public void Cancel_Should_ReturnFailure_When_StatusIsNotReserved()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);
        booking.CheckIn(UtcNow); // Status is CheckedIn

        // Act
        var result = booking.Cancel(UtcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.AlreadyStarted);
    }

    [Fact]
    public void Cancel_Should_SetStatusToCancelled_When_StatusIsReserved()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);

        // Act
        var result = booking.Cancel(UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancelledOnUtc.Should().Be(UtcNow);
    }

    [Fact]
    public void Cancel_Should_RaiseBookingCancelledDomainEvent()
    {
        // Arrange
        var booking = Booking.Reserve(RoomId, UserId, Duration, UtcNow, Price);

        // Act
        booking.Cancel(UtcNow);

        // Assert
        booking.GetDomainEvents().Should().ContainSingle(e => e is BookingCanceledDomainEvent);
    }
}