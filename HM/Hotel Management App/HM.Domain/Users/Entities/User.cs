using HM.Domain.Abstractions;
using HM.Domain.Users.Value_Objects;

namespace HM.Domain.Users.Entities;

public sealed class User : Entity
{
    public User(Guid id, Name name, ContactInfo contact, DateOnly dateOfBirth) : base(id)
    {
        Name = name;
        Contact = contact;
        DateOfBirth = dateOfBirth;
    }

    #region Properties
    public Name Name { get; init; }
    public ContactInfo Contact { get; init; }
    public DateOnly DateOfBirth { get; init ; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
    #endregion
}