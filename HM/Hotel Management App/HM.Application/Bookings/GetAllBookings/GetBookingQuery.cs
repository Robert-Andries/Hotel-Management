using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.Shared;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.GetAllBookings;

/// <summary>
///     Query to retrieve all bookings with an optional filter for completed ones.
/// </summary>
/// <param name="SeeCompletedBookings">Indicates whether to include completed bookings in the result.</param>
public sealed record GetAllBookingsQuery(bool SeeCompletedBookings = false) : IQuery<Result<List<BookingResponse>>>;