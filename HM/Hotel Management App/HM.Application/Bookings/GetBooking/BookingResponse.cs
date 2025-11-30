namespace HM.Application.Bookings.GetBooking;

public sealed record BookingResponse(
    Guid Id,
    Guid UserId,
    Guid RoomId,
    int Status,
    decimal PriceAmount,
    string PriceCurrency,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTime CreatedOnUtc);