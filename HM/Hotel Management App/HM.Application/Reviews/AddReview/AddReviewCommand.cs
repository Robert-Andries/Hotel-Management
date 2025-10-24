using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Application.Reviews.AddReview;

public record AddReviewCommand(Guid RoomId, Guid UserId, Comment Comment, int Rating) : ICommand<Result> 
{}