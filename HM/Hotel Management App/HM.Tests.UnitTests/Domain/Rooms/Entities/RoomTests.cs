using FluentAssertions;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Events;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using Xunit;

namespace HM.Tests.UnitTests.Domain.Rooms.Entities;

public class RoomTests
{
    private static readonly Money DefaultPrice = new(100, Currency.Usd);
    private static readonly RoomType DefaultType = RoomType.Single;
    private static readonly RoomLocation DefaultLocation = new(1, 101);
    private static readonly List<Feautre> DefaultFeatures = new(); // Typo in Domain 'Feautre' matches source

    [Fact]
    public void Create_Should_ReturnFailure_When_PriceIsZeroOrNegative()
    {
        // Arrange
        var price = new Money(0, Currency.Usd);

        // Act
        var result = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, price);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.InvalidPrice);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_PriceIsPositive()
    {
        // Act
        var result = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Status.Should().Be(RoomStatus.Available);
        result.Value.GetDomainEvents().Should().ContainSingle(e => e is RoomCreatedDomainEvent);
    }

    [Fact]
    public void Reserve_Should_ReturnFailure_When_RoomIsNotAvailable()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        room.Occupy(); // Status becomes Occupied

        // Act
        var result = room.Reserve(DateTime.UtcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.NotAvailable);
    }

    [Fact]
    public void Reserve_Should_SetStatusToReserved_When_RoomIsAvailable()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        var now = DateTime.UtcNow;

        // Act
        var result = room.Reserve(now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Status.Should().Be(RoomStatus.Reserved);
        room.LastBookedOnUtc.Should().Be(now);
    }

    [Fact]
    public void Occupy_Should_ReturnFailure_When_RoomIsOccupiedOrMaintenance()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        room.Occupy(); // Already Occupied

        // Act
        var result = room.Occupy();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.NotAvailable);
    }

    [Fact]
    public void Occupy_Should_SetStatusToOccupied_When_RoomIsAvailableOrReserved()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        room.Reserve(DateTime.UtcNow); // Reserved

        // Act
        var result = room.Occupy();

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Status.Should().Be(RoomStatus.Occupied);
    }

    [Fact]
    public void ReleaseForMaintenance_Should_ReturnFailure_When_RoomIsNotOccupied()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        // Status is Available

        // Act
        var result = room.ReleaseForMaintenance();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.NotOccupied);
    }

    [Fact]
    public void ReleaseForMaintenance_Should_SetStatusToMaintenance_When_RoomIsOccupied()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        room.Occupy();

        // Act
        var result = room.ReleaseForMaintenance();

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Status.Should().Be(RoomStatus.Maintanance);
    }

    [Fact]
    public void ResetStatus_Should_SetStatusToAvailable()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        room.Occupy();
        room.ReleaseForMaintenance(); // Status is Maintenance

        // Act
        var result = room.ResetStatus();

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Status.Should().Be(RoomStatus.Available);
    }

    [Fact]
    public void UpdateRating_Should_UpdateRatingAndRaiseEvent()
    {
        // Arrange
        var room = Room.Create(DefaultType, DefaultLocation, DefaultFeatures, DefaultPrice).Value;
        room.ClearDomainEvents();
        var newRating = new RatingSummary(room.Id, 4.5f, 10);

        // Act
        var result = room.UpdateRating(newRating);

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Rating.Should().Be(newRating);
        room.GetDomainEvents().Should().ContainSingle(e => e is RoomRatingUpdatedDomainEvent);
    }
}