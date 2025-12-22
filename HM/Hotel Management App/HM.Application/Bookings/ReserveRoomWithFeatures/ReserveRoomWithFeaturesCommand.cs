using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Bookings.ReserveRoomWithFeatures;

public sealed record ReserveRoomWithFeaturesCommand(
    Guid UserId,
    DateOnly StartDate,
    DateOnly EndDate,
    List<Feautre> RequiredFeatures) : ICommand<Result<Guid>>;
