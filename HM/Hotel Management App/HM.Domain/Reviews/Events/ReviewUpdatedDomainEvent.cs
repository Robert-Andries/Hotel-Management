using HM.Domain.Abstractions;

namespace HM.Domain.Reviews.Events;

public sealed record ReviewUpdatedDomainEvent(Guid ReviewId, Guid RoomId) : IDomainEvent;