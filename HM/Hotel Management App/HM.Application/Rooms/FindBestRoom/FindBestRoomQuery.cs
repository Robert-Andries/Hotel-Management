using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.GetAvailableRooms;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Rooms.FindBestRoom;

public sealed record FindBestRoomQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    RoomType RoomType,
    List<Feature> RequiredFeatures) : IQuery<Result<RoomSearchResponse>>;