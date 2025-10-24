using HM.Domain.Abstractions;

namespace HM.Domain.Bookings;

public class BookingErrors
{
    public static Error Overlaping = new Error(
        "Booking.Overlap",
        "The given booking is overlapping with another booking.");
    
    public static Error NotFound = new Error(
        "Booking.NotFound",
        "The given booking does not exist.");
    
}