using MediatR;

namespace HM.Application.Abstractions.Messaging;

/// <summary>
///     Represents a command that does not return a value.
/// </summary>
public interface ICommand : IRequest, IBaseCommand
{
}

/// <summary>
///     Represents a command that returns a value of type <typeparamref name="TResponse" />.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand
{
}

/// <summary>
///     Marker interface for all commands.
/// </summary>
public interface IBaseCommand
{
}