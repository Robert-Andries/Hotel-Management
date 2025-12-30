using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Application.Rooms.AddRoom;

/// <summary>
///     Command to add a new room to the system.
/// </summary>
/// <param name="Type">The type of room (e.g., Standard, Suite).</param>
/// <param name="Location">The physical location of the room.</param>
/// <param name="Feautres">List of features/amenities available in the room.</param>
/// <param name="Price">The nightly price of the room.</param>
public record AddRoomCommand(
    RoomType Type,
    RoomLocation Location,
    List<Feature> Feautres,
    Money Price) : ICommand<Result>;