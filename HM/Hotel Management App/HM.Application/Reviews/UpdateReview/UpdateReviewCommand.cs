using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Application.Reviews.UpdateReview;

/// <summary>
///     Command to update an existing review.
/// </summary>
/// <param name="ReviewId">The unique identifier of the review.</param>
/// <param name="Rating">The new rating.</param>
/// <param name="Comment">The new comment content.</param>
public sealed record UpdateReviewCommand(Guid ReviewId, int Rating, Comment Comment) : ICommand<Result>;