using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : ICommand<Result>;