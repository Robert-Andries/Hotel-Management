using FluentAssertions;
using HM.Application.Users.AddUser;
using HM.Application.Users.Services;
using HM.Domain.Abstractions;
using HM.Domain.Users;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Users.AddUser;

public class AddUserCommandHandlerTests
{
    private readonly AddUserCommandHandler _handler;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public AddUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _timeMock = new Mock<ITime>();

        var userCreationService = new UserCreationService(_timeMock.Object, _userRepositoryMock.Object);

        _handler = new AddUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            userCreationService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_EmailIsUnique()
    {
        // Arrange
        var command = new AddUserCommand(
            "John",
            "Doe",
            "123456789",
            "+1",
            "john.doe@example.com",
            new DateOnly(1990, 1, 1));

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        _userRepositoryMock
            .Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _userRepositoryMock.Verify(x => x.Add(It.Is<User>(u => u.Contact.Email.ToString() == command.Email)),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmailIsNotUnique()
    {
        // Arrange
        var command = new AddUserCommand(
            "John",
            "Doe",
            "123456789",
            "+1",
            "john.doe@example.com",
            new DateOnly(1990, 1, 1));

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        _userRepositoryMock
            .Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailNotUnique);

        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserCreationFails()
    {
        // Arrange
        var command = new AddUserCommand(
            "John",
            "Doe",
            "123456789",
            "+1",
            "invalid-email", // Invalid Email
            new DateOnly(1990, 1, 1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidEmail);

        _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
    }
}