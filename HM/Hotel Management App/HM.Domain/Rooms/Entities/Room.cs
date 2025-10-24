using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Rooms.Entities;

public sealed class Room : Entity
{
    public Room(Guid id, RoomType roomType, RoomLocation location, 
        List<Feautre> features, RatingSummary rating, RoomStatus status, Money price, DateTime? lastBookedOnUtc = null) : base(id)
    {
        RoomType = roomType;
        Location = location;
        Features = features;
        Rating = rating;
        Status = status;
        Price = price;
        LastBookedOnUtc = lastBookedOnUtc;
    }

    #region Properties
    public RoomType RoomType { get; private set; }
    public RoomLocation Location { get; private set; }
    public List<Feautre> Features { get; private set; }
    public RatingSummary Rating { get; set; }
    public RoomStatus Status { get; set; }
    public Money Price { get; set; }
    public DateTime? LastBookedOnUtc { get; set; }
    #endregion
}