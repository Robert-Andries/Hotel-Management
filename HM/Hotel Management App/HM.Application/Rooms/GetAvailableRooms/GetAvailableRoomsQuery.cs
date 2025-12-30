using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Rooms.GetAvailableRooms;

/// <summary>
///     Query to search for available rooms within a date range and required features.
/// </summary>
/// <param name="StartDate">Start date.</param>
/// <param name="EndDate">End date.</param>
/// <param name="RequiredFeatures">List of features required.</param>
public sealed record GetAvailableRoomsQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    List<Feature> RequiredFeatures) : IQuery<Result<List<RoomSearchResponse>>>;