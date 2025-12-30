using System.Text.RegularExpressions;
using HM.Domain.Abstractions;

namespace HM.Domain.Users.Value_Objects;

/// <summary>
///     Represents a validated phone number with country code.
/// </summary>
public record PhoneNumber
{
    private PhoneNumber(string value, string countryCode)
    {
        Value = value;
        CountryCode = countryCode;
    }

    /// <summary>Gets the local number part.</summary>
    public string Value { get; init; }

    /// <summary>Gets the country dialing code (e.g. +1, +40).</summary>
    public string CountryCode { get; init; }

    /// <summary>
    ///     Creates a new <see cref="PhoneNumber" /> value object using regex validation.
    /// </summary>
    /// <param name="value">The local number string.</param>
    /// <param name="countryCode">The country code string.</param>
    /// <returns>A Result containing the validated PhoneNumber or failure.</returns>
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

    public override string ToString()
    {
        return $"{CountryCode} {Value}";
    }
}