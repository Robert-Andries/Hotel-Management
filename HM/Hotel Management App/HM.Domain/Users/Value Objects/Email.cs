namespace HM.Domain.Users.Value_Objects;

public record Email(string Value, string Domain)
{
    private Email() : this(null!, null!) { }
}