namespace HM.Domain.Shared;

/// <summary>
///     Represents a supported currency in the system.
/// </summary>
public record Currency()
{
    /// <summary>Currency placeholder for none.</summary>
    internal static readonly Currency None = new("");

    /// <summary>Romanian Leu.</summary>
    public static readonly Currency Ron = new("RON");

    /// <summary>United States Dollar.</summary>
    public static readonly Currency Usd = new("USD");

    /// <summary>Euro.</summary>
    public static readonly Currency Eur = new("EUR");

    /// <summary>
    ///     Gets a collection of all supported currencies.
    /// </summary>
    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Ron,
        Usd,
        Eur
    };

    private Currency(string code) : this()
    {
        Code = code;
    }

    /// <summary>
    ///     Gets the ISO currency code.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    ///     Retrieves a currency instance from its ISO code.
    /// </summary>
    /// <param name="code">The ISO currency code.</param>
    /// <returns>The matching <see cref="Currency" />.</returns>
    /// <exception cref="ApplicationException">Thrown if the code is invalid.</exception>
    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ??
               throw new ApplicationException("The currency code is invalid");
    }
}