using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.GetBooking;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.GetAllBookings;

public sealed record GetAllBookingsQuery : IQuery<Result<List<BookingResponse>>>;