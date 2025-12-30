using HM.Domain.Abstractions;

namespace HM.Infrastructure.DateTimeProvider;

/// <summary>
///     Provides access to the current system time.
/// </summary>
public class Time : ITime
{
    public DateTime NowUtc => DateTime.Now;
}