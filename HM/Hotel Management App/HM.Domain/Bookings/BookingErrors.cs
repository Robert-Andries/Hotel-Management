using HM.Domain.Abstractions;

namespace HM.Domain.Bookings;

public class BookingErrors
{
    public static Error Overlapping = new(
        "Booking.Overlap",
        "The given booking is overlapping with another booking.");

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