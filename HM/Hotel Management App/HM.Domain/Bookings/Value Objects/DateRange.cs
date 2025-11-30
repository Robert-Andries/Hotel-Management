using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Value_Objects;

public record DateRange
{
    private DateRange()
    {
    }

    public DateOnly Start { get; init; }

    public DateOnly End { get; init; }

    public int LengthInDays => End.DayNumber - Start.DayNumber;

    public static Result<DateRange> Create(DateOnly start, DateOnly end)
    {
        if (start >= end)
            return Result.Failure<DateRange>(new Error(
                "DateRange.Invalid",
                "End date precedes start date"));

        DateRange output = new()
        {
            Start = start,
            End = end
        };

        return Result.Success(output);
    }
}