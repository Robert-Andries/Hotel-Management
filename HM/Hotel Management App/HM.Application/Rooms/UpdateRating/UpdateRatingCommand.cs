using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;

namespace HM.Application.Rooms.UpdateRating;

public record UpdateRatingCommand(Guid RoomId, RatingSummary NewRating) : ICommand<Result>;