using HM.Domain.Abstractions;
using HM.Domain.Booking.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Booking.Entities;

public class Booking : Entity
{
    public Booking(Guid id, Guid userId, Guid roomId, DateRange duration, Money price,
        DateTime createdOnUtc, BookingStatus status)
    : base(id)
    {
        UserId = userId;
        RoomId = roomId;
        Duration = duration;
        Price = price;
        CreatedOnUtc = createdOnUtc;
        Status = status;
    }

    public Guid UserId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateRange Duration { get; set; }
    public Money Price { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime? ConfirmedOnUtc { get; set; }
    public DateTime? RejectedOnUtc { get; set; }
    public DateTime? CancelledOnUtc { get; set; }
    public DateTime? CompletedOnUtc { get; set; }
    public DateTime? CheckedInOnUtc { get; set; }
    public DateTime? CheckedOutOnUtc { get; set; }
}