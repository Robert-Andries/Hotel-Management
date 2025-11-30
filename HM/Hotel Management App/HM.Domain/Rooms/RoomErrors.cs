using HM.Domain.Abstractions;

namespace HM.Domain.Rooms;

public class RoomErrors
{
    public static Error NotFound = new(
        "Room.NotFound",
        "The room with the specified id does not exist.");

    public static Error NotAvailable = new(
        "Room.NotAvailable",
        "The room is not available.");

    public static Error NotOccupied = new(
        "Room.NotOccupied",
        "The room is not occupied.");
}