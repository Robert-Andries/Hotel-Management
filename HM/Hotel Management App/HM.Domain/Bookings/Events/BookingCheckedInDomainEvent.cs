using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public record BookingCheckedInDomainEvent(Guid BookingId) : IDomainEvent;