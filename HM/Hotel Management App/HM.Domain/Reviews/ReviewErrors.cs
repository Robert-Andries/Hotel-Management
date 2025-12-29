using HM.Domain.Abstractions;

namespace HM.Domain.Reviews;

public static class ReviewErrors
{
    public static readonly Error NotFound = new("Review.NotFound",
        "The review with the specified identifier was not found.");

    public static readonly Error NotBeenInRoom = new("Review.NotBeenInRoom",
        "The given user is not in that room.");

    public static readonly Error BookingStatusNeedsToBeCompleted = new("Review.BookingStatusNeedsToBeCompleted",
        "To post a review the booking needs to be completed.");
}