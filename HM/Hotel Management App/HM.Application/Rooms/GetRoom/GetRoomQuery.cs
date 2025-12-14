using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Rooms.GetRoom;

public record GetRoomQuery(Guid RoomId) : IQuery<Result<RoomResponse>>;