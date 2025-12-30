namespace HM.Domain.Bookings.Value_Objects;

/// <summary>
///     Enumerates the possible states of a booking.
/// </summary>
public enum BookingStatus
{
    /// <summary>Confirmed reservation.</summary>
    Reserved,

    /// <summary>Guest has checked in.</summary>
    CheckedIn,

    /// <summary>Booking was cancelled.</summary>
    Cancelled,

    /// <summary>Guest checked out, booking complete.</summary>
    Completed
}