using FluentAssertions;
using HM.Domain.Abstractions;
using NetArchTest.Rules;
using Xunit;

namespace HM.Tests.ArchitecturalTests.Domain;

public class DomainTests
{
    [Fact]
    public void DomainEvents_Should_Be_Sealed()
    {
        var assembly = typeof(Entity).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entities_Should_Be_Sealed()
    {
        var assembly = typeof(Entity).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Entity))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}