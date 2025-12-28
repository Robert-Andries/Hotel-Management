using HM.Domain.Users.Entities;

namespace HM.Application.Users.Shared;

public record UserResponse(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber)
{
    public UserResponse(User user) : this(
        user.Id,
        user.Name.FirstName,
        user.Name.LastName,
        user.Contact.Email.ToString(),
        user.Contact.PhoneNumber.ToString())
    {
    }

    public string FullName => $"{FirstName} {LastName}";
}