using System.Runtime.InteropServices;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms.Events;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Rooms.Entities;

public sealed class Room : Entity
{
    private Room(Guid id, RoomType roomType, RoomLocation location,
        List<Feautre> features, RatingSummary rating, RoomStatus status, Money price) 
        : base(id)
    {
        RoomType = roomType;
        Location = location;
        Features = features;
        Rating = rating;
        Status = status;
        Price = price;
    }

    #region Properties
    public RoomType RoomType { get; private set; }
    public RoomLocation Location { get; private set; }
    public List<Feautre> Features { get; private set; }
    public RatingSummary Rating { get; private set; }
    public RoomStatus Status { get; private set; }
    public Money Price { get; private set; }
    public DateTime? LastBookedOnUtc { get; private set; }

    #endregion

    /// <summary>
    /// Factory method to create a Room entity
    /// </summary>
    /// <returns>The newly created room entity</returns>
    public static Result<Room> Create(RoomType roomType, RoomLocation location, List<Feautre> features, Money price)
    {
        if (price.Amount <= 0)
            return Result.Failure<Room>(RoomErrors.InvalidPrice);
        
        var roomId = Guid.NewGuid();
        var rating = new RatingSummary(roomId, 0.0f, 0);

        var room = new Room(roomId,
            roomType,
            location,
            features,
            rating,
            RoomStatus.Available,
            price);
        
        room.RaiseDomainEvent(new RoomCreatedDomainEvent(roomId));

        return Result.Success(room);
    }
    
    /// <summary>
    /// Reserves the room for a specific date.
    /// </summary>
    /// <param name="date">The date of the reservation.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result Reserve(DateTime date)
    {
        if (Status != RoomStatus.Available)
            return Result.Failure(RoomErrors.NotAvailable);

        Status = RoomStatus.Reserved;
        LastBookedOnUtc = date;
        
        return Result.Success();
    }

    /// <summary>
    /// Marks the room as occupied.
    /// </summary>
    /// <returns>Result indicating success or failure.</returns>
    public Result Occupy()
    {
        if (Status == RoomStatus.Occupied || Status == RoomStatus.Maintanance)
            return Result.Failure(RoomErrors.NotAvailable);
        
        Status = RoomStatus.Occupied;
        
        return Result.Success();
    }

    /// <summary>
    /// Releases the room for maintenance.
    /// </summary>
    /// <returns>Result indicating success or failure.</returns>
    public Result ReleaseForMaintenance()
    {
        if (Status != RoomStatus.Occupied)
            return Result.Failure(RoomErrors.NotOccupied);
        
        Status = RoomStatus.Maintanance;
        
        return Result.Success();
    }

    /// <summary>
    /// Resets the room status to Available.
    /// </summary>
    /// <returns>Result indicating success or failure.</returns>
    public Result ResetStatus()
    {
        Status = RoomStatus.Available;
        
        return Result.Success();
    }

    /// <summary>
    /// Updates the room's rating.
    /// </summary>
    /// <param name="newRating">The new rating summary.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result UpdateRating(RatingSummary newRating)
    {
        Rating = newRating;
        RaiseDomainEvent(new RoomRatingUpdatedDomainEvent(Id, newRating.Average));
        return Result.Success();
    }
}