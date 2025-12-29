using FluentAssertions;
using HM.Application.Users.AddUser;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;

namespace HM.Tests.IntegrationTests.Application.Users;

public class CreateUserTests : BaseIntegrationTest
{
    public CreateUserTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateUser_WhenCommandIsValid()
    {
        // Arrange
        var command = new AddUserCommand(
            "John",
            "Doe",
            "123456789",
            "+1",
            "john.doe@test.com",
            new DateOnly(1990, 1, 1));

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.ShouldBeSuccess();

        var user = await DbContext.Users.FindAsync(result.Value);
        user.Should().NotBeNull();
        user.Contact.Email.Value.Should().Be("john.doe");
        user.Contact.Email.Domain.Should().Be("test.com");
        user.Contact.PhoneNumber.CountryCode.Should().Be("+1");
        user.Contact.PhoneNumber.Value.Should().Be("123456789");
        user.Name.FirstName.Should().Be("John");
        user.Name.LastName.Should().Be("Doe");
        user.DateOfBirth.Should().Be(new DateOnly(1990, 1, 1));
    }
}