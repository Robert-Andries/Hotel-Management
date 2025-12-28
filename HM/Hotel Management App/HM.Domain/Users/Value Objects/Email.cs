using System.Net.Mail;
using HM.Domain.Abstractions;

namespace HM.Domain.Users.Value_Objects;

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

    public string Value { get; init; }
    public string Domain { get; init; }

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