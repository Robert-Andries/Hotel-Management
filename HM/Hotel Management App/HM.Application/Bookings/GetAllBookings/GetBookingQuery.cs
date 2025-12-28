using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.Shared;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.GetAllBookings;

public sealed record GetAllBookingsQuery(bool SeeCompletedBookings = false) : IQuery<Result<List<BookingResponse>>>;