using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Users.AddUser;

/// <summary>
///     Command to register a new user in the system.
/// </summary>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="PhoneNumber">The user's phone number as a string.</param>
/// <param name="CountryCode">The country code for the phone number.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="DateOfBirth">The user's date of birth.</param>
public record AddUserCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string CountryCode,
    string Email,
    DateOnly DateOfBirth) : ICommand<Result<Guid>>;