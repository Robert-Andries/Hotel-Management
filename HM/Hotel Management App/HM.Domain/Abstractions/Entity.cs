namespace HM.Domain.Abstractions;

/// <summary>
///     Represents the base class for all domain entities.
///     Encapsulates a unique identifier and domain events.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected Entity(Guid id)
    {
        Id = id;
    }

    // EF purposes
    protected Entity()
    {
    }

    /// <summary>
    ///     Gets the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Gets a read-only list of domain events raised by this entity.
    /// </summary>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    /// <summary>
    ///     Clears all domain events from the entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    ///     Raises a new domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}