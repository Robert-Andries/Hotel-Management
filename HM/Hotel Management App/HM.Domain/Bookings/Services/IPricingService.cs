using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;

namespace HM.Domain.Bookings.Services;

/// <summary>
///     Pricing service for calculating booking prices.
/// </summary>
public interface IPricingService
{
    /// <summary>
    ///     Calculates the full price breakdown for a room and duration.
    /// </summary>
    /// <param name="room">The room to be booked.</param>
    /// <param name="period">The duration of the stay.</param>
    /// <returns>A detailed pricing breakdown including base price, amenity charges, and total.</returns>
    PricingDetails CalculatePrice(Room room, DateRange? period = null);
}