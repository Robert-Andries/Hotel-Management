using HM.Domain.Shared;

namespace HM.Application.Bookings.GetBookingPreview;

public sealed record BookingPreviewResponse(
    Money PriceForPeriod,
    Money AmenitiesUpCharge,
    Money TotalPrice,
    string Currency);