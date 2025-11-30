using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CheckInGuest;

public sealed record CheckInGuestCommand(Guid BookingId) : ICommand<Result>;