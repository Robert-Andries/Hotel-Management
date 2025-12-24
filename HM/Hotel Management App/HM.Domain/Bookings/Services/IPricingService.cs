using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;

namespace HM.Domain.Bookings.Services;

public interface IPricingService
{
    PricingDetails CalculatePrice(Room room, DateRange? period = null);
}