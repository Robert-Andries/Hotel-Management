using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Rooms.ChangeStatus;

public record ChangeStatusCommand(Guid RoomId, RoomStatus Status) : ICommand<Result>;