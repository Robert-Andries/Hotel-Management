using FluentAssertions;
using HM.Domain.Shared;
using Xunit;

namespace HM.Tests.UnitTests.Domain.Shared;

public class MoneyTests
{
    [Fact]
    public void Constructor_Should_SetProperties()
    {
        var amount = 100m;
        var currency = Currency.Usd;

        var money = new Money(amount, currency);

        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Addition_Should_ReturnCorrectSum_When_CurrenciesMatch()
    {
        var m1 = new Money(100, Currency.Usd);
        var m2 = new Money(50, Currency.Usd);

        var result = m1 + m2;

        result.Amount.Should().Be(150);
        result.Currency.Should().Be(Currency.Usd);
    }

    [Fact]
    public void Addition_Should_ThrowException_When_CurrenciesMismatch()
    {
        var m1 = new Money(100, Currency.Usd);
        var m2 = new Money(50, Currency.Eur);

        var act = () => m1 + m2;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Currencies have to be equal");
    }

    [Fact]
    public void Zero_Should_ReturnMoneyWithZeroAmount()
    {
        var zero = Money.Zero();

        zero.Amount.Should().Be(0);
        zero.Currency.Should().Be(Currency.None);
    }
}