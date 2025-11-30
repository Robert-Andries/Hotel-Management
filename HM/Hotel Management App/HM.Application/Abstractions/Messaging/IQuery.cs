using MediatR;

namespace HM.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}