using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid BookingId) : IQuery<Result<BookingResponse>>;