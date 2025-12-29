using FluentAssertions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Tests.IntegrationTests.Infrastructure.Repositories;

public class BookingRepositoryTests : BaseIntegrationTest
{
    private readonly IBookingRepository _bookingRepository;

    public BookingRepositoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _bookingRepository = ServiceProvider.GetRequiredService<IBookingRepository>();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistBooking_WhenBookingIsValid()
    {
        // Arrange
        var booking = Booking.Reserve(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateRange.Create(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                .Value,
            DateTime.UtcNow,
            new Money(100, Currency.Usd));

        // Act
        var result = await _bookingRepository.AddAsync(booking);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var fromDb = await DbContext.Bookings.FindAsync(booking.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Id.Should().Be(booking.Id);
    }

    [Fact]
    public async Task IsOverlappingAsync_ShouldReturnTrue_WhenDatesOverlap()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var room = Room.Create(RoomType.Single, new RoomLocation(8, 801), new List<Feature>(),
            new Money(50, Currency.Usd)).Value;
        // Hack: Repository expects Room entity but uses ID mainly. 
        // We should add logic to match ID.
        // Actually IsOverlappingAsync uses room.Id.
        // But we need to ADD a booking first that overlaps.
        var existingBooking = Booking.Reserve(
            room.Id,
            Guid.NewGuid(),
            DateRange.Create(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
                .Value,
            DateTime.UtcNow,
            new Money(150, Currency.Usd));

        await _bookingRepository.AddAsync(existingBooking);

        var newRange = DateRange.Create(DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(2))).Value;

        // Act
        var result = await _bookingRepository.IsOverlappingAsync(room, newRange);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}