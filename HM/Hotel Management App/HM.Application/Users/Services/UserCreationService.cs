using HM.Domain.Abstractions;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;

namespace HM.Application.Users.Services;

public class UserCreationService
{
    private readonly ITime _time;
    private readonly IUserRepository _userRepository;

    public UserCreationService(ITime time, IUserRepository userRepository)
    {
        _time = time;
        _userRepository = userRepository;
    }

    public async Task<Result<User>> GetUserByEmailAsync(string emailString,
        CancellationToken cancellationToken = default)
    {
        var emailParts = emailString.Split('@');
        if (emailParts.Length != 2)
            return Result.Failure<User>(new Error("Email.Invalid", "Invalid email format"));

        var email = new Email(emailParts[0], emailParts[1]);

        return await _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public Result<User> CreateUser(
        string firstName,
        string lastName,
        string emailString,
        string phoneNumberString,
        string countryCode,
        DateOnly dateOfBirth)
    {
        var emailParts = emailString.Split('@');
        if (emailParts.Length != 2)
            return Result.Failure<User>(new Error("Email.Invalid", "Invalid email format"));

        var email = new Email(emailParts[0], emailParts[1]);

        var name = new Name(firstName, lastName);

        var phoneNumberResult = PhoneNumber.Create(phoneNumberString, countryCode);
        if (phoneNumberResult.IsFailure)
            return Result.Failure<User>(phoneNumberResult.Error);

        var contactInfo = new ContactInfo(email, phoneNumberResult.Value);

        var today = DateOnly.FromDateTime(_time.NowUtc);

        return User.Create(name, contactInfo, dateOfBirth, today);
    }
}