using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

// TODO: Разделить по реализаций и контрактов.

public interface IEventStore
{
    Task AppendEventAsync<TAggregate>(
        Guid aggregateId, 
        IEvent @event, 
        int? expectedVersion = null, 
        Func<StreamState, Task>? action = null,
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
        Func<StreamState, Task>? action = null,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new();
    Task StoreAsync<TAggregate>(
        ICollection<TAggregate> aggregates, 
        Func<StreamState, Task>? action = null,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new();

    Task<IEnumerable<StreamState>> GetEventsAsync(
        AggregateInfo info,
        CancellationToken cancellationToken = default);
}

public interface IStore : IAsyncDisposable
{
    Task AddAsync(
        StreamState stream, 
        CancellationToken cancellationToken = default);
    Task<IEnumerable<StreamState>> GetEventsAsync(
        AggregateInfo info, 
        CancellationToken cancellationToken = default);
}

public class AggregateInfo
{
    public required Guid Id { get; set; }
    public int? Version { get; set; }
    public DateTime? CreatedUtc { get; set; }
}