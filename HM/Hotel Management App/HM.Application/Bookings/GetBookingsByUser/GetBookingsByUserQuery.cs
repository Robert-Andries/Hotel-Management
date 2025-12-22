using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.GetBooking;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.GetBookingsByUser;

public sealed record GetBookingsByUserQuery(Guid UserId) : IQuery<Result<List<BookingResponse>>>;
