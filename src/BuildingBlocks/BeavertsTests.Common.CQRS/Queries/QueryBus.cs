using MediatR;

namespace BeaversTests.Common.CQRS.Queries;

public class QueryBus(IMediator mediator) : IQueryBus
{
    private readonly IMediator _mediator = mediator ?? throw new Exception($"Missing dependency '{nameof(IMediator)}'");

    public virtual async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return result;
    }
}