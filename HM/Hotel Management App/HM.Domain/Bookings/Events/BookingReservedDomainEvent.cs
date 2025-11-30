using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public record BookingReservedDomainEvent(Guid BookingId) : IDomainEvent;