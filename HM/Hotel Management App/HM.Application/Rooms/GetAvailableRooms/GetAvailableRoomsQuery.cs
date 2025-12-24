using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Rooms.GetAvailableRooms;

public sealed record GetAvailableRoomsQuery(DateOnly StartDate, DateOnly EndDate)
    : IQuery<Result<List<AvailableRoomResponse>>>;