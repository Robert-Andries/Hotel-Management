using HM.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HM.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.OwnsOne(u => u.Name);

        builder.OwnsOne(u => u.Contact, contact =>
        {
            contact.OwnsOne(c => c.Email);
            contact.OwnsOne(c => c.PhoneNumber);
        });
    }
}
