using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Entities;

namespace HM.Application.Rooms.GetAllRooms;

public record GetAllRoomsQuery() : IQuery<Result<List<RoomResponse>>>;