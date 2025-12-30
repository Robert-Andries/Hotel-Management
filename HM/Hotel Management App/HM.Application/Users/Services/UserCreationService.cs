using HM.Domain.Abstractions;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;

namespace HM.Application.Users.Services;

/// <summary>
///     Service responsible for handling user creation logic and domain validations.
/// </summary>
public class UserCreationService
{
    private readonly ITime _time;
    private readonly IUserRepository _userRepository;

    public UserCreationService(ITime time, IUserRepository userRepository)
    {
        _time = time;
        _userRepository = userRepository;
    }

    /// <summary>
    ///     Retrieves a user by their email address.
    /// </summary>
    /// <param name="emailString">The email address to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the User if found, or an error if invalid email or not found.</returns>
    public async Task<Result<User>> GetUserByEmailAsync(string emailString,
        CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(emailString);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);
        var email = emailResult.Value;

        return await _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    /// <summary>
    ///     Creates a new User instance with value object validation and business rules.
    /// </summary>
    /// <param name="firstName">User's first name.</param>
    /// <param name="lastName">User's last name.</param>
    /// <param name="emailString">User's email address.</param>
    /// <param name="phoneNumberString">User's phone number.</param>
    /// <param name="countryCode">Country code for the phone number.</param>
    /// <param name="dateOfBirth">User's date of birth.</param>
    /// <returns>A Result containing the newly created User, or Validation failures.</returns>
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