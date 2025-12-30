using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.GetAvailableRooms;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Rooms.FindBestRoom;

/// <summary>
///     Query to find the best available room matching criteria.
/// </summary>
/// <param name="StartDate">The start date of reproduction.</param>
/// <param name="EndDate">The end date of reproduction.</param>
/// <param name="RoomType">The desired room type.</param>
/// <param name="RequiredFeatures">List of mandatory features.</param>
public sealed record FindBestRoomQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    RoomType RoomType,
    List<Feature> RequiredFeatures) : IQuery<Result<RoomSearchResponse>>;