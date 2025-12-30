namespace HM.Domain.Users.Value_Objects;

/// <summary>
///     Represents a person's full name.
/// </summary>
/// <param name="FirstName">The first name.</param>
/// <param name="LastName">The last name.</param>
public record Name(string FirstName, string LastName)
{
    private Name() : this(null!, null!)
    {
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}