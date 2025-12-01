using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Application.Rooms.AddRoom;

public record AddRoomCommand(RoomType Type,
        RoomLocation Location,
        List<Feautre> Feautres,
        Money Price) : ICommand<Result>;