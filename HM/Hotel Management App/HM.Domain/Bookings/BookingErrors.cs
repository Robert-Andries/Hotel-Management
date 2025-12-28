using HM.Domain.Abstractions;

namespace HM.Domain.Bookings;

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
}