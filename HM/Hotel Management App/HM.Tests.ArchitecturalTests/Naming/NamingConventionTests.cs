using FluentAssertions;
using HM.Application;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using NetArchTest.Rules;
using Xunit;

namespace HM.Tests.ArchitecturalTests.Naming;

public class NamingConventionTests
{
    [Fact]
    public void CommandHandlers_Should_Have_CommandHandler_Suffix()
    {
        var assembly = typeof(DependencyInjection).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CommandHandlers_Should_Have_CommandHandler_Suffix_Generic()
    {
        var assembly = typeof(DependencyInjection).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryHandlers_Should_Have_QueryHandler_Suffix()
    {
        var assembly = typeof(DependencyInjection).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvents_Should_Have_DomainEvent_Suffix()
    {
        var assembly = typeof(Entity).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}