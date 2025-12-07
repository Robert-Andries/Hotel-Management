using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.GetAllRooms;
using HM.Domain.Abstractions;

namespace HM.Application.Rooms.GetRoom;

public record GetRoomQuery(Guid RoomId) : IQuery<Result<RoomResponse>>;