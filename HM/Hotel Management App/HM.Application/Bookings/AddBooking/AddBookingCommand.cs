using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.AddBooking;

public sealed record AddBookingCommand(Guid UserId, DateOnly StartDate, DateOnly EndDate, Guid RoomId)
    : ICommand<Result<Guid>>;