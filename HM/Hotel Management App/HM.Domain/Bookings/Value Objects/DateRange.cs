using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Value_Objects;

/// <summary>
///     Represents a range of dates (start and end).
/// </summary>
public record DateRange
{
    /// <summary>A default one-day range placeholder.</summary>
    public static DateRange OneDay = new()
    {
        Start = DateOnly.MinValue,
        End = DateOnly.MinValue.AddDays(1)
    };

    private DateRange()
    {
    }

    /// <summary>Gets the start date.</summary>
    public DateOnly Start { get; init; }

    /// <summary>
    ///     Gets the end date.
    /// </summary>
    public DateOnly End { get; init; }

    /// <summary>Gets the total length in days.</summary>
    public int LengthInDays => End.DayNumber - Start.DayNumber;

    /// <summary>
    ///     Creates a new DateRange.
    /// </summary>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <returns>A validated DateRange or an error if End precedes Start.</returns>
    public static Result<DateRange> Create(DateOnly start, DateOnly end)
    {
        if (start >= end)
            return Result.Failure<DateRange>(DateRangeErrors.InvalidDate);

        DateRange output = new()
        {
            Start = start,
            End = end
        };

        return Result.Success(output);
    }
}