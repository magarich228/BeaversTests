using MediatR;

namespace BeaversTests.Common.CQRS.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse> 
{ }