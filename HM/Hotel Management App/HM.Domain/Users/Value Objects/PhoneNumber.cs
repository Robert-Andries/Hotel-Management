using System.Text.RegularExpressions;
using HM.Domain.Abstractions;

namespace HM.Domain.Users.Value_Objects;

public record PhoneNumber
{
    private PhoneNumber(string value, string countryCode)
    {
        Value = value;
        CountryCode = countryCode;
    }

    public string Value { get; init; }
    public string CountryCode { get; init; }

    public static Result<PhoneNumber> Create(string value, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(countryCode))
            return Result.Failure<PhoneNumber>(UserErrors.InvalidPhoneNumber);

        // Validate Country Code (e.g., +1, +40) - starts with +, followed by 1-4 digits
        if (!Regex.IsMatch(countryCode, @"^\+\d{1,4}$"))
            return Result.Failure<PhoneNumber>(UserErrors.InvalidPhoneNumber);

        // Validate Phone Number - digits only, reasonable length (e.g. 3-15)
        if (!Regex.IsMatch(value, @"^\d{3,15}$")) return Result.Failure<PhoneNumber>(UserErrors.InvalidPhoneNumber);

        return Result.Success(new PhoneNumber(value, countryCode));
    }
}