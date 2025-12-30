using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Rooms.FinishMaintenance;

/// <summary>
///     Command to finish maintenance on a room and make it available again.
/// </summary>
/// <param name="RoomId">The ID of the room.</param>
public record FinishMaintenanceCommand(Guid RoomId) : ICommand<Result>;