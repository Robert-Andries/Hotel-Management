using HM.Domain.Abstractions;

namespace HM.Domain.Users;

public class UserErrors
{
    public static Error NotFound = new(
        "User.NotFound",
        "The user with the specified id was not found.");
}