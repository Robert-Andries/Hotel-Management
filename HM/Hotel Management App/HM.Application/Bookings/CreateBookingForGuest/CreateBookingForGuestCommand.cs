using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.CreateBookingForGuest;

/// <summary>
///     Command to create a new booking for a guest, registering the user if they don't exist.
/// </summary>
/// <param name="FirstName">The guest's first name.</param>
/// <param name="LastName">The guest's last name.</param>
/// <param name="Email">The guest's email address.</param>
/// <param name="PhoneNumber">The guest's phone number.</param>
/// <param name="CountryCode">The guest's country code.</param>
/// <param name="DateOfBirth">The guest's date of birth.</param>
/// <param name="StartDate">The booking start date (check-in).</param>
/// <param name="EndDate">The booking end date (check-out).</param>
/// <param name="RoomId">The ID of the room to book.</param>
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