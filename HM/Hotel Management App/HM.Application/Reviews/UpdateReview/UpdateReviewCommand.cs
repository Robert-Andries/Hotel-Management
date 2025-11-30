using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Application.Reviews.UpdateReview;

public sealed record UpdateReviewCommand(Guid ReviewId, int Rating, Comment Comment) : ICommand<Result>;
