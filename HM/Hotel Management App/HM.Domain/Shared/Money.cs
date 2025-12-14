namespace HM.Domain.Shared;

public sealed record Money(decimal Amount, Currency Currency)
{
    private Money() : this(0, Currency.None)
    {
    }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency) throw new InvalidOperationException("Currencies have to be equal");

        return first with { Amount = first.Amount + second.Amount };
    }

    public static Money Zero()
    {
        return new Money(0, Currency.None);
    }

    public static Money Zero(Currency currency)
    {
        return new Money(0, currency);
    }

    public bool IsZero()
    {
        return Amount == 0;
    }

    public override string ToString()
    {
        return $"{Amount} {Currency.Code}";
    }
}