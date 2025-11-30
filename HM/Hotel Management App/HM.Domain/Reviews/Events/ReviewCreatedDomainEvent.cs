using HM.Domain.Abstractions;

namespace HM.Domain.Reviews.Events;

public record ReviewCreatedDomainEvent(Guid ReviewId, Guid RoomId) : IDomainEvent;