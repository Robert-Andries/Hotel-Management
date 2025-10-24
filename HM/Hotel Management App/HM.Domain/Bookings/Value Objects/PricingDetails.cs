using HM.Domain.Shared;

namespace HM.Domain.Bookings.Value_Objects;

public record PricingDetails(
    Money PriceForPeriod,
    Money AmenitiesUpCharge,
    Money TotalPrice);