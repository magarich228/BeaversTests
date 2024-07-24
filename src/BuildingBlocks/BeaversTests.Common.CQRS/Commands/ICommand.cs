using MediatR;

namespace BeaversTests.Common.CQRS.Commands;

public interface ICommand<out TResponse> : IRequest<TResponse>
{ }

public interface ICommand : IRequest
{ }