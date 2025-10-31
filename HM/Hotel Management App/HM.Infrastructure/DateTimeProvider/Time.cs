using HM.Domain.Abstractions;

namespace HM.Infrastructure.DateTimeProvider;

public class Time : ITime
{
    public DateTime NowUtc => DateTime.Now;
}