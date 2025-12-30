using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Application.Reviews.AddReview;

/// <summary>
///     Command to add a review for a room.
/// </summary>
/// <param name="RoomId">The unique identifier of the room.</param>
/// <param name="UserId">The unique identifier of the user posting the review.</param>
/// <param name="Comment">The review content.</param>
/// <param name="Rating">The rating score.</param>
public sealed record AddReviewCommand(
    Guid RoomId,
    Guid UserId,
    Comment Comment,
    int Rating) : ICommand<Result>
{
}