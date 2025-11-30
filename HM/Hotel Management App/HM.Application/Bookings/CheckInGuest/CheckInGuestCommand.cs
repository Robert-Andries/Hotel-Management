using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CheckInGuest;

public record CheckInGuestCommand(Guid BookingId) : ICommand<Result>;