using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CheckOutGuest;

/// <summary>
///     Command to check out a guest from a booking.
/// </summary>
/// <param name="BookingId">The unique identifier of the booking.</param>
public sealed record CheckOutGuestCommand(Guid BookingId) : ICommand<Result>;