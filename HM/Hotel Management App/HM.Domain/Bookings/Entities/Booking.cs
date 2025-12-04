using HM.Domain.Abstractions;
using HM.Domain.Bookings.Events;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Bookings.Entities;

public sealed class Booking : Entity
{
    private Booking() { }

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

    #region Proprieties
    public Guid UserId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateRange Duration { get; private set; }
    public Money Price { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime ReservedOnUtc { get; private set; }
    public DateTime? CheckedInOnUtc { get; private set; }
    public DateTime? CancelledOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }
    #endregion

    /// <summary>
    /// Factory method for creating and reserving a booking
    /// </summary>
    /// <returns>The freshly created booking</returns>
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
    /// Updates Status to BookingStatus.CheckedIn
    /// </summary>
    /// <param name="checkInUtc">The time when the check in occured</param>
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
    /// Updates Status to BookingStatus.Completed
    /// </summary>
    /// <param name="checkOutUtc">The time when the check-out occured</param>
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
    /// Updates Status to BookingStatus.Cancelled
    /// </summary>
    /// <param name="cancelledUtc">The time when the cancel occured</param>
    public Result Cancel(DateTime cancelledUtc)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.AlreadyStarted);

        Status = BookingStatus.Cancelled;
        CancelledOnUtc = cancelledUtc;
        
        RaiseDomainEvent(new BookingCanceledDomainEvent(Id));
        
        return Result.Success();
    }
}