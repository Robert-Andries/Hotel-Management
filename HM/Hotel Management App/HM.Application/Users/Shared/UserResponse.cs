using HM.Domain.Users.Entities;

namespace HM.Application.Users.Shared;

/// <summary>
///     DTO representing a user response.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="PhoneNumber">The user's phone number.</param>
public record UserResponse(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber)
{
    /// <summary>
    ///     Initializes a new instance of <see cref="UserResponse" /> from a domain entity.
    /// </summary>
    /// <param name="user">The user entity.</param>
    public UserResponse(User user) : this(
        user.Id,
        user.Name.FirstName,
        user.Name.LastName,
        user.Contact.Email.ToString(),
        user.Contact.PhoneNumber.ToString())
    {
    }

    /// <summary>Gets the full name (First + Last).</summary>
    public string FullName => $"{FirstName} {LastName}";
}