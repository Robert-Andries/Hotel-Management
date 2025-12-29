using HM.Domain.Abstractions;

namespace HM.Domain.Rooms.Events;

public sealed record RoomRatingUpdatedDomainEvent(Guid RoomId, double NewRating) : IDomainEvent;