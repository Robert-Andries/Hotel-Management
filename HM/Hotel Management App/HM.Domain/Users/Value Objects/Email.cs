using System.Net.Mail;
using HM.Domain.Abstractions;

namespace HM.Domain.Users.Value_Objects;

/// <summary>
///     Represents a valid email address.
/// </summary>
public record Email
{
    private Email()
    {
        Value = string.Empty;
        Domain = string.Empty;
    }

    private Email(string value, string domain)
    {
        Value = value;
        Domain = domain;
    }

    /// <summary>Gets the user part of the email address (before @).</summary>
    public string Value { get; init; }

    /// <summary>Gets the domain part of the email address (after @).</summary>
    public string Domain { get; init; }

    /// <summary>
    ///     Creates a new <see cref="Email" /> value object.
    /// </summary>
    /// <param name="email">The raw email string.</param>
    /// <returns>A Result containing the Email object or a validation error.</returns>
    public static Result<Email> Create(string email)
    {
        if (!IsEmailValid(email))
            return Result.Failure<Email>(UserErrors.InvalidEmail);

        var emailParts = email.Split('@');
        var value = emailParts[0];
        var domain = emailParts[1];

        var outputemail = new Email(value, domain);

        return Result.Success<Email>(outputemail);
    }

    public override string ToString()
    {
        return $"{Value}@{Domain}";
    }

    private static bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        try
        {
            var addr = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}