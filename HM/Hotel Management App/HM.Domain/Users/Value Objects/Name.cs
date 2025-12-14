namespace HM.Domain.Users.Value_Objects;

public record Name(string FirstName, string LastName)
{
    private Name() : this(null!, null!)
    {
    }
}