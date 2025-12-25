using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Value_Objects;

public record DateRange
{
    public static DateRange OneDay = new()
    {
        Start = DateOnly.MinValue,
        End = DateOnly.MinValue.AddDays(1)
    };

    private DateRange()
    {
    }

    public DateOnly Start { get; init; }

    public DateOnly End { get; init; }

    public int LengthInDays => End.DayNumber - Start.DayNumber;

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