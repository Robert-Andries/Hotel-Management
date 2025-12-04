namespace HM.Domain.Users.Value_Objects;

public record PhoneNumber(string Value, string CountryCode)
{
    private PhoneNumber() : this(null!, null!) { }
}