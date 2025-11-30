using HM.Domain.Abstractions;

namespace HM.Domain.Bookings.Events;

public sealed record BookingCheckedInDomainEvent(Guid BookingId) : IDomainEvent;