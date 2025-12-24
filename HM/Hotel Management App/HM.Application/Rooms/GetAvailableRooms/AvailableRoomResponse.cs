using HM.Application.Rooms.GetRoom;
using HM.Domain.Rooms.Entities;
using HM.Domain.Shared;

namespace HM.Application.Rooms.GetAvailableRooms;

public class AvailableRoomResponse : RoomResponse
{
    public AvailableRoomResponse(Room room, Money totalPrice) : base(room)
    {
        TotalPrice = totalPrice;
    }

    public Money TotalPrice { get; }
}