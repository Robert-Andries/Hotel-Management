using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Application.Rooms.GetRoom;

public class RoomResponse
{
    public RoomResponse(Room room)
    {
        Id = room.Id;
        RoomType = room.RoomType;
        Location = room.Location;
        Features = room.Features;
        Rating = room.Rating;
        Status = room.Status;
        Price = room.Price;
        LastBookedOnUtc = room.LastBookedOnUtc;
    }

    public Guid Id { get; set; }
    public RoomType RoomType { get; private set; }
    public RoomLocation Location { get; private set; }
    public List<Feautre> Features { get; private set; }
    public RatingSummary Rating { get; private set; }
    public RoomStatus Status { get; private set; }
    public Money Price { get; private set; }
    public DateTime? LastBookedOnUtc { get; private set; }
}