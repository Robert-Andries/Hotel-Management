using HM.Application.Users.Shared;
using HM.Domain.Bookings.Value_Objects;

namespace HM.Application.Bookings.Shared;

/// <summary>
///     DTO representing a booking response.
/// </summary>
/// <param name="Id">The unique identifier of the booking.</param>
/// <param name="User">The user who made the booking.</param>
/// <param name="RoomId">The ID of the booked room.</param>
/// <param name="Location">The location of the room.</param>
/// <param name="Status">The current status of the booking.</param>
/// <param name="PriceAmount">The total price amount.</param>
/// <param name="PriceCurrency">The currency of the price.</param>
/// <param name="StartDate">The check-in date.</param>
/// <param name="EndDate">The check-out date.</param>
/// <param name="CreatedOnUtc">The UTC timestamp when the booking was created.</param>
public sealed record BookingResponse(
    Guid Id,
    UserResponse User,
    Guid RoomId,
    string Location,
    BookingStatus Status,
    decimal PriceAmount,
    string PriceCurrency,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTime CreatedOnUtc);