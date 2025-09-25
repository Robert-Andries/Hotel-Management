namespace HM.Domain.Reviews.Value_Objects;

public sealed record RatingSummary(decimal Average, int Count)
{
    public RatingSummary Add(int rating)
    {
        var newCount = Count + 1;
        var newAvg = ((Average * Count) + rating) / newCount;
        return this with { Average = newAvg, Count = newCount };
    }

    public RatingSummary Replace(int oldRating, int newRating)
    {
        if (Count == 0) return this;
        var newAvg = ((Average * Count) - oldRating + newRating) / Count;
        return this with { Average = newAvg };
    }

    public RatingSummary Remove(int rating)
    {
        if (Count <= 1) return new RatingSummary(0, 0);
        var newCount = Count - 1;
        var newAvg = ((Average * Count) - rating) / newCount;
        return this with { Average = newAvg, Count = newCount };
    }
}
