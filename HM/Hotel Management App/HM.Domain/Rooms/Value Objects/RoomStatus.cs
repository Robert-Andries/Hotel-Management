namespace HM.Domain.Rooms.Value_Objects;

/// <summary>
///     Enumerates the possible states of a room.
/// </summary>
public enum RoomStatus
{
    /// <summary>Ready for booking.</summary>
    Available,

    /// <summary>Booked by a guest.</summary>
    Reserved,

    /// <summary>Currently occupied by a guest.</summary>
    Occupied,

    /// <summary>Under maintenance/cleaning.</summary>
    Maintanance
}