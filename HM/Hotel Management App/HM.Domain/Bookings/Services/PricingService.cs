using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;

namespace HM.Domain.Bookings.Services;

public class PricingService
{
    public PricingDetails CalculatePrice(Room apartment, DateRange period)
    {
        var currency = apartment.Price.Currency;

        var priceForPeriod = new Money(
            apartment.Price.Amount * period.LengthInDays,
            currency);

        decimal percentageUpCharge = 0;
        foreach (var amenity in apartment.Features)
            percentageUpCharge += amenity switch
            {
                Feautre.GardenView or Feautre.MountainView => 0.05m,
                Feautre.AirConditioning => 0.01m,
                Feautre.Parking => 0.01m,
                Feautre.Tv => 0.01m,
                Feautre.Terrace => 0.02m,
                Feautre.PetFriendly => 0.03m,
                Feautre.Balcony => 0.02m,
                Feautre.WiFi => 0.01m,
                _ => 0
            };

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