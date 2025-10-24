using HM.Domain.Abstractions;

namespace HM.Domain.Shared;

public class Time : ITime
{
    public DateTime NowUtc => DateTime.UtcNow;
}