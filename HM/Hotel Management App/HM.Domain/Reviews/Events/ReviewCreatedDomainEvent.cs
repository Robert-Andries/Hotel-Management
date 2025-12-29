using HM.Domain.Abstractions;

namespace HM.Domain.Reviews.Events;

public sealed record ReviewCreatedDomainEvent(Guid ReviewId, Guid RoomId) : IDomainEvent;