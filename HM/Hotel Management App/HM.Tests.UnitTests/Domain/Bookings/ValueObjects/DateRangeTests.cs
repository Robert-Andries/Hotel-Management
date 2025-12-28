using FluentAssertions;
using HM.Domain.Bookings.Value_Objects;
using Xunit;

namespace HM.Tests.UnitTests.Domain.Bookings.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void Create_Should_ReturnFailure_When_StartIsGreaterThanEnd()
    {
        // Arrange
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var end = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = DateRange.Create(start, end);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DateRangeErrors.InvalidDate);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_StartIsLessThanEnd()
    {
        // Arrange
        var start = DateOnly.FromDateTime(DateTime.UtcNow);
        var end = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act
        var result = DateRange.Create(start, end);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Start.Should().Be(start);
        result.Value.End.Should().Be(end);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_StartIsEqualToEnd()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = DateRange.Create(date, date);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DateRangeErrors.InvalidDate);
    }

    [Fact]
    public void LengthInDays_Should_ReturnCorrectNumberOfDays()
    {
        // Arrange
        var start = new DateOnly(2023, 1, 1);
        var end = new DateOnly(2023, 1, 5);
        var dateRange = DateRange.Create(start, end).Value;

        // Act
        var length = dateRange.LengthInDays;

        // Assert
        length.Should().Be(4);
    }
}