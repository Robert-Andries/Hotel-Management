using HM.Domain.Abstractions;

namespace HM.Domain.Users;

public class UserErrors
{
    public static Error NotFound = new(
        "User.NotFound",
        "The user with the specified id was not found.");
    
    public static Error InvalidEmail = new Error(
        "User.InvalidEmail",
        "The provided email is invalid.");

    public static Error EmailNotUnique = new Error(
        "User.EmailNotUnique",
        "The provided email is already in use.");
}