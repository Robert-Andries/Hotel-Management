using HM.Domain.Abstractions;
using HM.Domain.Users.Value_Objects;

namespace HM.Domain.Users.Entities;

/// <summary>
///     Represents a user (guest or staff) in the system.
/// </summary>
public sealed class User : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private User(Guid id, Name name, ContactInfo contact, DateOnly dateOfBirth) : base(id)
    {
        Name = name;
        Contact = contact;
        DateOfBirth = dateOfBirth;
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="User" /> entity.
    /// </summary>
    /// <param name="name">The user's full name.</param>
    /// <param name="contact">The user's contact information.</param>
    /// <param name="dateOfBirth">The user's date of birth.</param>
    /// <param name="today">The current date for age validation.</param>
    /// <returns>A Result containing the newly created User or an error if validation fails.</returns>
    public static Result<User> Create(Name name, ContactInfo contact, DateOnly dateOfBirth, DateOnly today)
    {
        var user = new User(Guid.NewGuid(), name, contact, dateOfBirth);

        var age = user.GetAge(today);
        if (age < 18 || age > 110) return Result.Failure<User>(UserErrors.InvalidAge);

        return Result.Success(user);
    }

    #region Properties

    /// <summary>Gets the user's name.</summary>
    public Name Name { get; init; }

    /// <summary>Gets the user's contact details.</summary>
    public ContactInfo Contact { get; init; }

    /// <summary>Gets the user's date of birth.</summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    ///     Calculates the user's age based on a reference date.
    /// </summary>
    /// <param name="today">The reference date.</param>
    /// <returns>The calculated age in years.</returns>
    public int GetAge(DateOnly today)
    {
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth > today.AddYears(-age))
            age--;
        return age;
    }

    #endregion
}