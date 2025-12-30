namespace HM.Domain.Users.Value_Objects;

/// <summary>
///     Represents aggregated contact information for a user.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="PhoneNumber">The user's phone number.</param>
public record ContactInfo(Email Email, PhoneNumber PhoneNumber)
{
    private ContactInfo() : this(null!, null!)
    {
    }
}