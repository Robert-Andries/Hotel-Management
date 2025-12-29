using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Bookings.Services;

public sealed class PricingService : IPricingService
{
    public PricingDetails CalculatePrice(Room apartment, DateRange? period = null)
    {
        var currency = apartment.Price.Currency;
        period ??= DateRange.OneDay;

        var priceForPeriod = new Money(
            apartment.Price.Amount * period.LengthInDays,
            currency);

        var percentageUpCharge = apartment.Features.Sum(amenity => amenity switch
        {
            Feature.GardenView or Feature.MountainView => 0.05m,
            Feature.AirConditioning => 0.01m,
            Feature.Parking => 0.01m,
            Feature.Tv => 0.01m,
            Feature.Terrace => 0.02m,
            Feature.PetFriendly => 0.03m,
            Feature.Balcony => 0.02m,
            Feature.WiFi => 0.01m,
            _ => 0
        });

        var amenitiesUpCharge = Money.Zero(currency);
        if (percentageUpCharge > 0)
            amenitiesUpCharge = new Money(
                priceForPeriod.Amount * percentageUpCharge,
                currency);

        var totalPrice = Money.Zero(currency);
        totalPrice += priceForPeriod;
        totalPrice += amenitiesUpCharge;

        return new PricingDetails(priceForPeriod, amenitiesUpCharge, totalPrice);
    }
}