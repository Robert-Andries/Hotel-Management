using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.GetAvailableRooms;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Rooms.FindAvailableRoom;

public sealed record FindAvailableRoomQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    RoomType RoomType,
    List<Feautre> RequiredFeatures) : IQuery<Result<RoomSearchResponse>>;