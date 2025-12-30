using HM.Domain.Abstractions;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms.Events;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Rooms.Entities;

/// <summary>
///     Represents a room within the hotel.
/// </summary>
public sealed class Room : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Room()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Room(Guid id, RoomType roomType, RoomLocation location,
        List<Feature> features, RatingSummary rating, RoomStatus status, Money price)
        : base(id)
    {
        RoomType = roomType;
        Location = location;
        Features = features;
        Rating = rating;
        Status = status;
        Price = price;
    }

    /// <summary>
    ///     Factory method to create a new Room entity.
    /// </summary>
    /// <param name="roomType">The type of the room (Single, Double, etc.).</param>
    /// <param name="location">The location (floor, number).</param>
    /// <param name="features">List of amenities.</param>
    /// <param name="price">Price per night.</param>
    /// <returns>The newly created room entity or failure if price is invalid.</returns>
    public static Result<Room> Create(RoomType roomType, RoomLocation location, List<Feature> features, Money price)
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
    ///     Reserves the room for a specific date.
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
    ///     Marks the room as occupied.
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
    ///     Releases the room for maintenance.
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
    ///     Resets the room status to Available.
    /// </summary>
    /// <returns>Result indicating success or failure.</returns>
    public Result ResetStatus()
    {
        Status = RoomStatus.Available;

        return Result.Success();
    }

    /// <summary>
    ///     Updates the room's rating based on new reviews.
    /// </summary>
    /// <param name="newRating">The new rating summary.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result UpdateRating(RatingSummary newRating)
    {
        Rating = newRating;
        RaiseDomainEvent(new RoomRatingUpdatedDomainEvent(Id, newRating.Average));
        return Result.Success();
    }

    #region Properties

    /// <summary>Gets the type of the room.</summary>
    public RoomType RoomType { get; private set; }

    /// <summary>Gets the physical location of the room.</summary>
    public RoomLocation Location { get; private set; }

    /// <summary>Gets the list of features (amenities).</summary>
    public List<Feature> Features { get; private set; }

    /// <summary>Gets the aggregate rating summary.</summary>
    public RatingSummary Rating { get; private set; }

    /// <summary>Gets the current status (Available, Reserved, etc.).</summary>
    public RoomStatus Status { get; private set; }

    /// <summary>Gets the price per night.</summary>
    public Money Price { get; private set; }

    /// <summary>Gets the UTC timestamp of the last booking.</summary>
    public DateTime? LastBookedOnUtc { get; private set; }

    #endregion
}