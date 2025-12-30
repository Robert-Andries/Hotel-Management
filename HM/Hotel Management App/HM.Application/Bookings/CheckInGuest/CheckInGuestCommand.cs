using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CheckInGuest;

/// <summary>
///     Command to check in a guest for an existing booking.
/// </summary>
/// <param name="BookingId">The unique identifier of the booking.</param>
public sealed record CheckInGuestCommand(Guid BookingId) : ICommand<Result>;