namespace HM.Domain.Rooms.Value_Objects;

public record RoomLocation(int Floor, int RoomNumber)
{
    private RoomLocation() : this(0, 0) { }
}