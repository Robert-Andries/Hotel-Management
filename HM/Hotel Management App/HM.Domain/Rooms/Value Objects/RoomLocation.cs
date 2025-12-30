namespace HM.Domain.Rooms.Value_Objects;

/// <summary>
///     Represents the physical location of a room.
/// </summary>
/// <param name="Floor">The floor number.</param>
/// <param name="RoomNumber">The room number.</param>
public record RoomLocation(int Floor, int RoomNumber)
{
    private RoomLocation() : this(0, 0)
    {
    }

    public override string ToString()
    {
        return $"Floor : {Floor}, RoomNumber: {RoomNumber}";
    }
}