using HM.Domain.Abstractions;

namespace HM.Domain.Reviews.Events;

public record ReviewUpdatedDomainEvent(Guid ReviewId, Guid RoomId) : IDomainEvent;
