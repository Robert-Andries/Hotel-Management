namespace HM.Domain.Users.Value_Objects;

public record ContactInfo(Email Email, PhoneNumber PhoneNumber)
{
    private ContactInfo() : this(null!, null!) { }
}