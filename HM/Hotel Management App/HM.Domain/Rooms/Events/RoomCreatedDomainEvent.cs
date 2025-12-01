using HM.Domain.Abstractions;

namespace HM.Domain.Rooms.Events;

public record RoomCreatedDomainEvent(Guid RoomId) : IDomainEvent;