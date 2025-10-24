using HM.Domain.Abstractions;

namespace HM.Domain.Rooms;

public class RoomErrors
{
    public static Error NotFound = new(
        "Room.NotFound",
        "The room with the specified id does not exist.");
}