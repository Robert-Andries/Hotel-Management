using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CreateBookingForGuest;

public sealed record CreateBookingForGuestCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string CountryCode,
    DateOnly DateOfBirth,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid RoomId) : ICommand<Result<Guid>>;