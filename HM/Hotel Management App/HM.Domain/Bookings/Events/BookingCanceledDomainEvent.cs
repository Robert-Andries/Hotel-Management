using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public sealed record BookingCanceledDomainEvent(Guid BookingId) : IDomainEvent;