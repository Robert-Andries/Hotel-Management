using HM.Domain.Users.Entities;

namespace HM.Application.Users.GetUsers;

public record UserResponse(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber)
{
    public UserResponse(User user) : this(
        user.Id,
        user.Name.FirstName,
        user.Name.LastName,
        user.Contact.Email.Value,
        user.Contact.PhoneNumber.Value)
    {
    }
}
