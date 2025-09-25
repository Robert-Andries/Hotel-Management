using HM.Domain.Shared;
using HM.Domain.Room.Entities;

namespace HM.Domain.Room.Interfaces;

public interface IPricingService
{
    Money CalculatePrice(Entities.Room room, int numberOfNights);
}