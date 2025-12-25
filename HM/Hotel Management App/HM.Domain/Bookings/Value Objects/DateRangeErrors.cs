using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Value_Objects;

public static class DateRangeErrors
{
    public static Error InvalidDate = new(
        "DateRange.Invalid",
        "End date precedes start date.");
}