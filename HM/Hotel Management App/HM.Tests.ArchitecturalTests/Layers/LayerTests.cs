using FluentAssertions;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Infrastructure;
using NetArchTest.Rules;
using Xunit;

namespace HM.Tests.ArchitecturalTests.Layers;

public class LayerTests
{
    private const string DomainNamespace = "HM.Domain";
    private const string ApplicationNamespace = "HM.Application";
    private const string InfrastructureNamespace = "HM.Infrastructure";
    private const string PresentationNamespace = "HM.Presentation.WebUI";

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Other_Layers()
    {
        var assembly = typeof(Entity).Assembly;

        var otherLayers = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            PresentationNamespace
        };

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Infrastructure_Or_Presentation()
    {
        var assembly = typeof(ICommandHandler<>).Assembly;

        var otherLayers = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace
        };

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_Have_Dependency_On_Presentation()
    {
        var assembly = typeof(DependencyInjection).Assembly;

        var otherLayers = new[]
        {
            PresentationNamespace
        };

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}