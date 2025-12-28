using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Users.AddUser;

public record AddUserCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string CountryCode,
    string Email,
    DateOnly DateOfBirth) : ICommand<Result<Guid>>;