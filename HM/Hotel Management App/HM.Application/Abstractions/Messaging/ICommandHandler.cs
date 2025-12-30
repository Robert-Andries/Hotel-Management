using MediatR;

namespace HM.Application.Abstractions.Messaging;

/// <summary>
///     Defines a handler for a command that does not return a value.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled.</typeparam>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

/// <summary>
///     Defines a handler for a command that returns a value.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}