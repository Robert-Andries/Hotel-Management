using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Rooms.FinishMaintenance;

public record FinishMaintenanceCommand(Guid  RoomId) : ICommand<Result>;