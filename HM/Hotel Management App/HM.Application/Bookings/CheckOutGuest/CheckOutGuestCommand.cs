using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CheckOutGuest;

public record CheckOutGuestCommand(Guid BookingId) : ICommand<Result>;