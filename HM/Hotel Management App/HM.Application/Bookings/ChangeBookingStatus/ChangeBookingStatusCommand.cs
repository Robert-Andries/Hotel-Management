using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Value_Objects;

namespace HM.Application.Bookings.ChangeBookingStatus;

public record ChangeBookingStatusCommand(Guid BookingId, BookingStatus Status) : ICommand<Result>;