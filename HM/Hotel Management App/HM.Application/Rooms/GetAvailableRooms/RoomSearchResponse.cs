using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Application.Rooms.GetAvailableRooms;

/// <summary>
///     DTO representing a room found in a search result.
/// </summary>
/// <param name="RoomId">Unique room ID.</param>
/// <param name="RoomType">Type of the room.</param>
/// <param name="PricePerNight">Price per night.</param>
/// <param name="TotalPrice">Total price for the duration.</param>
/// <param name="Features">List of features.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Location">Physical location.</param>
public sealed record RoomSearchResponse(
    Guid RoomId,
    RoomType RoomType,
    Money PricePerNight,
    Money TotalPrice,
    List<Feature> Features,
    string Currency,
    RoomLocation Location);