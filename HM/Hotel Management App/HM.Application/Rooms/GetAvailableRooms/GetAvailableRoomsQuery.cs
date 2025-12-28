using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Rooms.GetAvailableRooms;

public sealed record GetAvailableRoomsQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    List<Feautre> RequiredFeatures) : IQuery<Result<List<RoomSearchResponse>>>;