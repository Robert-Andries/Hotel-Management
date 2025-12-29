using FluentAssertions;
using HM.Domain.Abstractions;

namespace HM.Tests.IntegrationTests.Extensions;

public static class ResultExtensions
{
    public static void ShouldBeSuccess(this Result result)
    {
        result.IsSuccess.Should().BeTrue($"Expected Success but got Failure: {result.Error}");
    }

    public static void ShouldBeFailure(this Result result, Error error)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    public static void ShouldBeSuccess<T>(this Result<T> result)
    {
        result.IsSuccess.Should().BeTrue($"Expected Success but got Failure: {result.Error}");
    }

    public static void ShouldBeFailure<T>(this Result<T> result, Error error)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}