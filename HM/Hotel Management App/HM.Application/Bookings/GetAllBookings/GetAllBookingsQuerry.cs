using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Entities;

namespace HM.Application.Bookings.GetAllBookings;

public record GetAllBookingsQuerry() : IQuery<Result<List<Booking>>>;