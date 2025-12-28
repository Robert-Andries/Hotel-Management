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
        var emailResult = Email.Create(emailString);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);
        var email = emailResult.Value;

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
        var emailResult = Email.Create(emailString);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);
        var email = emailResult.Value;

        var name = new Name(firstName, lastName);

        var phoneNumberResult = PhoneNumber.Create(phoneNumberString, countryCode);
        if (phoneNumberResult.IsFailure)
            return Result.Failure<User>(phoneNumberResult.Error);
        var phoneNumber = phoneNumberResult.Value;

        var contactInfo = new ContactInfo(email, phoneNumber);

        var today = DateOnly.FromDateTime(_time.NowUtc);

        return User.Create(name, contactInfo, dateOfBirth, today);
    }
}