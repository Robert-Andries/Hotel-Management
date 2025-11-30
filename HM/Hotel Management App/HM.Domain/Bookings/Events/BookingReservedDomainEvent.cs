using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public sealed record BookingReservedDomainEvent(Guid BookingId) : IDomainEvent;