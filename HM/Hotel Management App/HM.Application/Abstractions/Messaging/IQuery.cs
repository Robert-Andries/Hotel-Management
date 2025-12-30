using MediatR;

namespace HM.Application.Abstractions.Messaging;

/// <summary>
///     Represents a query operation that retrieves data of type <typeparamref name="TResponse" />.
/// </summary>
/// <typeparam name="TResponse">The type of data returned.</typeparam>
public interface IQuery<TResponse> : IRequest<TResponse>
{
}