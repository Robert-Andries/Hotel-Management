using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Room.Interfaces;
using HM.Domain.Room.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Room.Entities;

public sealed class Room : Entity
{
    public Room(Guid id, IPricingService pricingService, RoomType roomType, RoomLocation location, 
        List<Feautre> features, RatingSummary rating, RoomStatus status, DateTime? lastBookedOnUtc = null) : base(id)
    {
        _pricingService = pricingService;
        RoomType = roomType;
        Location = location;
        Features = features;
        Rating = rating;
        Status = status;
        LastBookedOnUtc = lastBookedOnUtc;
    }

    #region Properties
    public RoomType RoomType { get; private set; }
    public RoomLocation Location { get; private set; }
    public List<Feautre> Features { get; private set; }
    public RatingSummary Rating { get; internal set; }
    public RoomStatus Status { get; private set; }
    public DateTime? LastBookedOnUtc { get; internal set; }
    #endregion
    
    #region Methods
    public Money CalculatePrice(int numberOfNights) => _pricingService.CalculatePrice(this, numberOfNights);
    #endregion
    
    private readonly IPricingService _pricingService;
}