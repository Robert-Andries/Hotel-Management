namespace HM.Domain.Shared;

/// <summary>
///     Represents a monetary value consisting of an amount and a currency.
/// </summary>
/// <param name="Amount">The decimal amount.</param>
/// <param name="Currency">The currency type.</param>
public sealed record Money(decimal Amount, Currency Currency)
{
    private Money() : this(0, Currency.None)
    {
    }

    /// <summary>
    ///     Adds two Money instances.
    /// </summary>
    /// <param name="first">First money amount.</param>
    /// <param name="second">Second money amount.</param>
    /// <returns>A new Money instance with the summed amount.</returns>
    /// <exception cref="InvalidOperationException">Thrown if currencies do not match.</exception>
    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency) throw new InvalidOperationException("Currencies have to be equal");

        return first with { Amount = first.Amount + second.Amount };
    }

    /// <summary>
    ///     Returns a zero amount with no currency.
    /// </summary>
    public static Money Zero()
    {
        return new Money(0, Currency.None);
    }

    /// <summary>
    ///     Returns a zero amount in the specified currency.
    /// </summary>
    public static Money Zero(Currency currency)
    {
        return new Money(0, currency);
    }

    /// <summary>
    ///     Checks if the amount is zero.
    /// </summary>
    public bool IsZero()
    {
        return Amount == 0;
    }

    public override string ToString()
    {
        return $"{Amount} {Currency.Code}";
    }
}