using HM.Domain.Abstractions;

namespace HM.Domain.Bookings;

/// <summary>
///     Contains predefined domain errors for the Booking context.
/// </summary>
public class BookingErrors
{
    public static Error Overlapping = new(
        "Booking.Overlap",
        "This room has just been booked by another user for the selected dates. Please search again.");

    public static Error NotFound = new(
        "Booking.NotFound",
        "The given booking does not exist.");

    public static Error NotReserved = new(
        "Booking.NotReserved",
        "The booking is not pending reservation.");

    public static Error NotCheckedIn = new(
        "Booking.NotCheckedIn",
        "The booking is not checked in.");

    public static Error AlreadyStarted = new(
        "Booking.AlreadyStarted",
        "The booking has already started.");

    public static Error CanNotBookInThePast = new(
        "Booking.CanNotBookInThePast",
        "You cannot create a booking with date range in the past. The booking date must be in the future.");
}