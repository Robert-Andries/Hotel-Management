using HM.Domain.Abstractions;

namespace HM.Domain.Rooms.Events;

public sealed record RoomCreatedDomainEvent(Guid RoomId) : IDomainEvent;