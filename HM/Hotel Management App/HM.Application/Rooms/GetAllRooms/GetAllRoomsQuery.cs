using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.GetRoom;
using HM.Domain.Abstractions;

namespace HM.Application.Rooms.GetAllRooms;

public record GetAllRoomsQuery : IQuery<Result<List<RoomResponse>>>;