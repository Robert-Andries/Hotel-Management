using HM.Application.Users.GetUsers;
using HM.Domain.Bookings.Value_Objects;

namespace HM.Application.Bookings.GetBooking;

public sealed record BookingResponse(
    Guid Id,
    UserResponse User,
    Guid RoomId,
    BookingStatus Status,
    decimal PriceAmount,
    string PriceCurrency,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTime CreatedOnUtc);