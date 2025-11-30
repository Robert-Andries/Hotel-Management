using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CancelBooking;

public record CancelBookingCommand(Guid BookingId) : ICommand<Result>;