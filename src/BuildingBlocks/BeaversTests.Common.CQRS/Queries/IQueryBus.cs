namespace BeaversTests.Common.CQRS.Queries;

public interface IQueryBus
{
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default(CancellationToken));
}