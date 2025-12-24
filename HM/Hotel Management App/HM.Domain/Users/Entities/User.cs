using HM.Domain.Abstractions;
using HM.Domain.Users.Value_Objects;

namespace HM.Domain.Users.Entities;

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

    public static Result<User> Create(Name name, ContactInfo contact, DateOnly dateOfBirth, DateOnly today)
    {
        var user = new User(Guid.NewGuid(), name, contact, dateOfBirth);

        var age = user.GetAge(today);
        if (age < 18 || age > 110) return Result.Failure<User>(UserErrors.InvalidAge);

        return Result.Success(user);
    }

    #region Properties

    public Name Name { get; init; }
    public ContactInfo Contact { get; init; }
    public DateOnly DateOfBirth { get; init; }

    public int GetAge(DateOnly today)
    {
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth > today.AddYears(-age)) age--;
        return age;
    }

    #endregion
}