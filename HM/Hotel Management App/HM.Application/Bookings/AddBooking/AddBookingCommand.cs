using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;
using HM.Domain.Users.Entities;

namespace HM.Application.Bookings.AddBooking;

public record AddBookingCommand(Guid UserId , DateRange Range, Guid RoomId) : ICommand<Result<Guid>>;