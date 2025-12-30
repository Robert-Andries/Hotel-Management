using HM.Domain.Shared;

namespace HM.Domain.Bookings.Value_Objects;

/// <summary>
///     Encapsulates the breakdown of a booking price.
/// </summary>
/// <param name="PriceForPeriod">Base price for the room over the duration.</param>
/// <param name="AmenitiesUpCharge">Extra charges for active amenities.</param>
/// <param name="TotalPrice">The final total price.</param>
public record PricingDetails(
    Money PriceForPeriod,
    Money AmenitiesUpCharge,
    Money TotalPrice);