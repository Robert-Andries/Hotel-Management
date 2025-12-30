namespace HM.Domain.Abstractions;

/// <summary>
///     Abstraction for accessing the current time.
///     Useful for testing time-dependent logic.
/// </summary>
public interface ITime
{
    /// <summary>
    ///     Gets the current UTC date and time.
    /// </summary>
    DateTime NowUtc { get; }
}