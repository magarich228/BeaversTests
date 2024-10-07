using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

// TODO: Разделить по реализаций и контрактов.

public interface IEventStore
{
    Task AppendEvent<TAggregate>(Guid aggregateId, IEvent @event, int? expectedVersion = null, Func<StreamState, Task>? action = null) where TAggregate : IAggregate;

    Task<TAggregate> AggregateStream<TAggregate>(AggregateInfo info) where TAggregate : IAggregate;
    Task<ICollection<TAggregate>> AggregateStream<TAggregate>(ICollection<Guid> ids) where TAggregate : IAggregate;

    Task Store<TAggregate>(TAggregate aggregate, Func<StreamState, Task>? action = null) where TAggregate : IAggregate;
    Task Store<TAggregate>(ICollection<TAggregate> aggregates, Func<StreamState, Task>? action = null) where TAggregate : IAggregate;

    Task<IEnumerable<StreamState>> GetEvents(Guid aggregateId, AggregateInfo info);
}

public interface IStore : IAsyncDisposable
{
    Task AddAsync(StreamState stream, CancellationToken cancellationToken = default);
    Task<IEnumerable<StreamState>> GetEventsAsync(AggregateInfo info, CancellationToken cancellationToken = default);
}

public class AggregateInfo
{
    public required Guid Id { get; set; }
    public int? Version { get; set; }
    public DateTime? CreatedUtc { get; set; }
}