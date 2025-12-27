using HM.Domain.Abstractions;

namespace HM.Domain.Reviews;

public static class ReviewErrors
{
    public static readonly Error NotFound = new("Review.NotFound",
        "The review with the specified identifier was not found.");
}