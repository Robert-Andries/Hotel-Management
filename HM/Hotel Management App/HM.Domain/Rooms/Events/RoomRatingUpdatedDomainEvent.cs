using HM.Domain.Abstractions;

namespace HM.Domain.Rooms.Events;

public record RoomRatingUpdatedDomainEvent(Guid RoomId, double NewRating) : IDomainEvent;