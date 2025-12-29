using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Application.Rooms.GetAvailableRooms;

public sealed record RoomSearchResponse(
    Guid RoomId,
    RoomType RoomType,
    Money PricePerNight,
    Money TotalPrice,
    List<Feature> Features,
    string Currency,
    RoomLocation Location);