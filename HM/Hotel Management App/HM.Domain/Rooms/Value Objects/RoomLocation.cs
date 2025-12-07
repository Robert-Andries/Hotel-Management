namespace HM.Domain.Rooms.Value_Objects;

public record RoomLocation(int Floor, int RoomNumber)
{
    private RoomLocation() : this(0, 0) { }

    public override string ToString()
    {
        return $"Floor : {Floor}, RoomNumber: {RoomNumber}";
    }
}