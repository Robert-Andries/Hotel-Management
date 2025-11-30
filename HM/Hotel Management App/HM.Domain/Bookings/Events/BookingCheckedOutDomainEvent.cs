using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public record BookingCheckedOutDomainEvent(Guid BookingId) : IDomainEvent;