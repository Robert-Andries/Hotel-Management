using MediatR;

namespace HM.Domain.Abstractions;

/// <summary>
///     Marker interface for domain events.
///     Inherits from <see cref="INotification" /> to integrate with MediatR.
/// </summary>
public interface IDomainEvent : INotification
{
}