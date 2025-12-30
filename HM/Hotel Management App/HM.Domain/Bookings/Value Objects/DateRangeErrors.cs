using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Value_Objects;

/// <summary>
///     Domain errors specific to DateRange logic.
/// </summary>
public static class DateRangeErrors
{
    public static Error InvalidDate = new(
        "DateRange.Invalid",
        "End date precedes start date.");
}