using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public record BookingCanceledDomainEvent(Guid BookingId) : IDomainEvent;