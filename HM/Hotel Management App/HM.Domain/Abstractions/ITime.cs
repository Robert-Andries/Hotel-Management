namespace HM.Domain.Abstractions;

public interface ITime
{
    DateTime NowUtc { get; }
}