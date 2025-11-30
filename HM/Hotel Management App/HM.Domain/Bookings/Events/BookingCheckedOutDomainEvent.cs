using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public sealed record BookingCheckedOutDomainEvent(Guid BookingId) : IDomainEvent;