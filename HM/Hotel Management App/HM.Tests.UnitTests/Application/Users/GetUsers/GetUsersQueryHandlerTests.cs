using FluentAssertions;
using HM.Application.Abstractions.Data;
using HM.Application.Users.GetUsers;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;
using HM.Tests.UnitTests.Helpers;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Users.GetUsers;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetUsersQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAllUsers_When_NoFilterProvided()
    {
        // Arrange
        var users = new List<User>
        {
            CreateUser("John", "Doe", "john@example.com"),
            CreateUser("Jane", "Doe", "jane@example.com")
        };
        var dbSet = MockDbSetHelper.GetQueryableMockDbSet(users);
        _contextMock.Setup(x => x.Users).Returns(dbSet);

        var query = new GetUsersQuery(null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnFilteredUsers_When_SearchTermProvided()
    {
        // Arrange
        var users = new List<User>
        {
            CreateUser("John", "Doe", "john@example.com"),
            CreateUser("Alice", "Smith", "alice@example.com")
        };
        var dbSet = MockDbSetHelper.GetQueryableMockDbSet(users);
        _contextMock.Setup(x => x.Users).Returns(dbSet);

        var filter = new UserFilter("John", null, null); // Search for "John"
        var query = new GetUsersQuery(filter);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().FirstName.Should().Be("John");
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedUsers_When_PaginationProvided()
    {
        // Arrange
        var users = new List<User>();
        for (var i = 1; i <= 10; i++) users.Add(CreateUser($"User{i}", "Test", $"user{i}@test.com"));

        var dbSet = MockDbSetHelper.GetQueryableMockDbSet(users);
        _contextMock.Setup(x => x.Users).Returns(dbSet);

        var filter = new UserFilter(null, 2, 3); // Page 2, Size 3 (Items 4, 5, 6)
        var query = new GetUsersQuery(filter);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.First().FirstName.Should().Be("User4");
        result.Value.Last().FirstName.Should().Be("User6");
    }

    private static User CreateUser(string firstName, string lastName, string email)
    {
        return User.Create(
            new Name(firstName, lastName),
            new ContactInfo(Email.Create(email).Value, PhoneNumber.Create("123456789", "+1").Value),
            new DateOnly(1990, 1, 1),
            DateOnly.FromDateTime(DateTime.UtcNow)
        ).Value;
    }
}