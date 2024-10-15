namespace BeaversTests.Common.CQRS.Abstractions;

public interface IEventStore
{
    Task AppendEventAsync<TAggregate>(
        Guid aggregateId, 
        IEvent @event, 
        int? expectedVersion = null,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new();

    Task<TAggregate> AggregateStreamAsync<TAggregate>(
        AggregateInfo info,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new();
    IAsyncEnumerable<TAggregate> AggregateStreamAsync<TAggregate>(
        ICollection<Guid> ids,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new();

    Task StoreAsync<TAggregate>(
        TAggregate aggregate,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new();
    
    Task StoreAsync<TAggregate>(
        ICollection<TAggregate> aggregates,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new();

    Task<IEnumerable<StreamState>> GetEventsAsync(
        AggregateInfo info,
        CancellationToken cancellationToken = default);
}