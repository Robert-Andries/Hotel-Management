using HM.Domain.Abstractions;
using HM.Domain.Bookings.Events;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Bookings.Entities;

/// <summary>
///     Represents a reservation of a room by a user.
/// </summary>
public sealed class Booking : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Booking()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Booking(Guid id, Guid userId, Guid roomId, DateRange duration, Money price,
        DateTime reservedOnUtc, BookingStatus status)
        : base(id)
    {
        UserId = userId;
        RoomId = roomId;
        Duration = duration;
        Price = price;
        ReservedOnUtc = reservedOnUtc;
        Status = status;
    }

    /// <summary>
    ///     Factory method for creating and reserving a booking.
    /// </summary>
    /// <param name="roomId">The ID of the room being booked.</param>
    /// <param name="userId">The ID of the user booking the room.</param>
    /// <param name="duration">The duration of the stay.</param>
    /// <param name="utcNow">The booking creation timestamp.</param>
    /// <param name="price">The total price for the booking.</param>
    /// <returns>The freshly created booking with Reserved status.</returns>
    public static Booking Reserve(Guid roomId, Guid userId, DateRange duration, DateTime utcNow, Money price)
    {
        var booking = new Booking(
            Guid.NewGuid(),
            userId,
            roomId,
            duration,
            price,
            utcNow,
            BookingStatus.Reserved);

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        return booking;
    }

    /// <summary>
    ///     Updates Status to BookingStatus.CheckedIn.
    /// </summary>
    /// <param name="checkInUtc">The time when the check in occurred.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result CheckIn(DateTime checkInUtc)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.NotReserved);

        Status = BookingStatus.CheckedIn;
        CheckedInOnUtc = checkInUtc;

        RaiseDomainEvent(new BookingCheckedInDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    ///     Updates Status to BookingStatus.Completed.
    /// </summary>
    /// <param name="checkOutUtc">The time when the check-out occurred.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result CheckOut(DateTime checkOutUtc)
    {
        if (Status != BookingStatus.CheckedIn)
            return Result.Failure(BookingErrors.NotCheckedIn);

        Status = BookingStatus.Completed;
        CompletedOnUtc = checkOutUtc;

        RaiseDomainEvent(new BookingCheckedOutDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    ///     Updates Status to BookingStatus.Cancelled.
    /// </summary>
    /// <param name="cancelledUtc">The time when the cancellation occurred.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result Cancel(DateTime cancelledUtc)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.AlreadyStarted);

        Status = BookingStatus.Cancelled;
        CancelledOnUtc = cancelledUtc;

        RaiseDomainEvent(new BookingCanceledDomainEvent(Id));

        return Result.Success();
    }

    #region Proprieties

    /// <summary>Gets the ID of the user who made the booking.</summary>
    public Guid UserId { get; private set; }

    /// <summary>Gets the ID of the booked room.</summary>
    public Guid RoomId { get; private set; }

    /// <summary>Gets the duration of the booking.</summary>
    public DateRange Duration { get; private set; }

    /// <summary>Gets the total price of the booking.</summary>
    public Money Price { get; private set; }

    /// <summary>Gets the current status of the booking.</summary>
    public BookingStatus Status { get; private set; }

    /// <summary>Gets the timestamp when the booking was created.</summary>
    public DateTime ReservedOnUtc { get; private set; }

    /// <summary>Gets the timestamp when the guest checked in.</summary>
    public DateTime? CheckedInOnUtc { get; private set; }

    /// <summary>Gets the timestamp when the booking was cancelled.</summary>
    public DateTime? CancelledOnUtc { get; private set; }

    /// <summary>Gets the timestamp when the guest checked out.</summary>
    public DateTime? CompletedOnUtc { get; private set; }

    #endregion
}