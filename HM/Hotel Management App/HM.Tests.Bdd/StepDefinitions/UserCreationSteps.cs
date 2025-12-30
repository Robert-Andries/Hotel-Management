using FluentAssertions;
using HM.Application.Users.Services;
using HM.Domain.Abstractions;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using Moq;
using Reqnroll;

namespace HM.Tests.Bdd.StepDefinitions;

[Binding]
public class UserCreationSteps
{
    private readonly Mock<ITime> _timeMock;
    private readonly UserCreationService _userCreationService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private string _countryCode;
    private DateOnly _dateOfBirth;
    private string _email;

    private string _firstName;
    private string _lastName;
    private string _phoneNumber;

    private Result<User> _result;

    public UserCreationSteps()
    {
        _timeMock = new Mock<ITime>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userCreationService = new UserCreationService(_timeMock.Object, _userRepositoryMock.Object);

        _timeMock.Setup(t => t.NowUtc).Returns(new DateTime(2023, 1, 1));
    }

    [Given("I have valid user data")]
    public void GivenIHaveValidUserData()
    {
        _firstName = "John";
        _lastName = "Doe";
        _email = "john.doe@example.com";
        _phoneNumber = "123456789";
        _countryCode = "+1";
        _dateOfBirth = new DateOnly(1990, 1, 1);
    }

    [Given("I have user data with an invalid email")]
    public void GivenIHaveUserDataWithAnInvalidEmail()
    {
        _firstName = "John";
        _lastName = "Doe";
        _email = "invalid-email";
        _phoneNumber = "123456789";
        _countryCode = "+1";
        _dateOfBirth = new DateOnly(1990, 1, 1);
    }

    [When("I request to create a user")]
    public void WhenIRequestToCreateAUser()
    {
        _result = _userCreationService.CreateUser(
            _firstName,
            _lastName,
            _email,
            _phoneNumber,
            _countryCode,
            _dateOfBirth);
    }

    [Then("the user should be created successfully")]
    public void ThenTheUserShouldBeCreatedSuccessfully()
    {
        _result.IsSuccess.Should().BeTrue();
    }

    [Then("the user details should match the input data")]
    public void ThenTheUserDetailsShouldMatchTheInputData()
    {
        _result.Value.Should().NotBeNull();
        _result.Value.Name.FirstName.Should().Be(_firstName);
        _result.Value.Name.LastName.Should().Be(_lastName);
        _result.Value.Contact.Email.ToString().Should().Be(_email);
    }

    [Then("the user creation should fail")]
    public void ThenTheUserCreationShouldFail()
    {
        _result.IsFailure.Should().BeTrue();
    }

    [Then("an error should be returned")]
    public void ThenAnErrorShouldBeReturned()
    {
        _result.Error.Should().NotBeNull();
    }
}