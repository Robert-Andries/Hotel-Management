using FluentAssertions;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Tests.IntegrationTests.Infrastructure.Repositories;

public class UserRepositoryTests : BaseIntegrationTest
{
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
    }

    [Fact]
    public async Task Add_ShouldPersistUser_WhenUserIsValid()
    {
        // Arrange
        var user = User.Create(
            new Name("Repo", "Tester"),
            new ContactInfo(Email.Create("repo@test.com").Value, PhoneNumber.Create("123123123", "+1").Value),
            new DateOnly(1990, 1, 1),
            DateOnly.FromDateTime(DateTime.Today)).Value;

        // Act
        _userRepository.Add(user);
        await DbContext.SaveChangesAsync();

        // Assert
        var fromDb = await DbContext.Users.FindAsync(user.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        // Arrange
        var emailStr = "email@check.com";
        var user = User.Create(
            new Name("Email", "Check"),
            new ContactInfo(Email.Create(emailStr).Value, PhoneNumber.Create("987654321", "+1").Value),
            new DateOnly(1995, 5, 5),
            DateOnly.FromDateTime(DateTime.Today)).Value;

        _userRepository.Add(user);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByEmailAsync(Email.Create(emailStr).Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task IsEmailUniqueAsync_ShouldReturnFalse_WhenEmailExists()
    {
        // Arrange
        var emailStr = "unique@check.com";
        var user = User.Create(
            new Name("Unique", "Check"),
            new ContactInfo(Email.Create(emailStr).Value, PhoneNumber.Create("111111111", "+1").Value),
            new DateOnly(2000, 1, 1),
            DateOnly.FromDateTime(DateTime.Today)).Value;

        _userRepository.Add(user);
        await DbContext.SaveChangesAsync();

        // Act
        var isUnique = await _userRepository.IsEmailUniqueAsync(Email.Create(emailStr).Value);

        // Assert
        isUnique.Should().BeFalse();
    }
}