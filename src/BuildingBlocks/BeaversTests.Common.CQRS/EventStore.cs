using System.Text.Json;
using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

public class EventStore(IStore store) : IEventStore
{
    private readonly IStore _store = store;

    public async Task AppendEventAsync<TAggregate>(
        Guid aggregateId, 
        IEvent @event, 
        int? expectedVersion = null, 
        Func<StreamState, Task>? action = null,
        CancellationToken cancellationToken = default) 
        where TAggregate : IAggregate
    {
        var version = 1;

        var events = await GetEventsAsync(new AggregateInfo()
        {
            Id = aggregateId
        }, cancellationToken);
        
        var versions = events.Select(e => e.Version).ToList();

        if (expectedVersion.HasValue)
        {
            if (versions.Contains(expectedVersion.Value))
            {
                // TODO: custom exception
                throw new Exception($"Version '{expectedVersion.Value}' already exists for stream '{aggregateId}'");
            }
        }
        else
        {
            version = versions.DefaultIfEmpty(0).Max() + 1;
        }

        var stream = new StreamState
        {
            AggregateId = aggregateId,
            Version = version,
            Type = GetQueueName(@event.GetType()),
            Data = JsonSerializer.Serialize(@event)
        };

        await _store.AddAsync(stream, cancellationToken);

        if (action != null)
        {
            await action(stream);
        }
    }

    public async Task<TAggregate> AggregateStreamAsync<TAggregate>(
        AggregateInfo info, 
        CancellationToken cancellationToken = default) 
        where TAggregate : IAggregate
    {
        var events = await GetEventsAsync(info, cancellationToken);

        // var aggregate = new TAggregate();

        foreach (var @event in events)
        {
            // TODO: custom exception
            var eventData = JsonSerializer.Deserialize<IEvent>(@event.Data) ??
                            throw new ApplicationException();
            // aggregate.Apply(eventData);

            // aggregate.Version += 1;
            // aggregate.CreatedUtc = @event.CreatedUtc;
        }

        throw new NotImplementedException();
        // return aggregate;
    }

    public Task<ICollection<TAggregate>> AggregateStreamAsync<TAggregate>(
        ICollection<Guid> ids, 
        CancellationToken cancellationToken = default) 
        where TAggregate : IAggregate
    {
        throw new NotImplementedException();
    }

    public Task StoreAsync<TAggregate>(
        TAggregate aggregate, 
        Func<StreamState, 
            Task>? action = null, 
        CancellationToken cancellationToken = default) 
        where TAggregate : IAggregate
    {
        throw new NotImplementedException();
    }

    public Task StoreAsync<TAggregate>(
        ICollection<TAggregate> aggregates, 
        Func<StreamState, Task>? action = null, 
        CancellationToken cancellationToken = default) 
        where TAggregate : IAggregate
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<StreamState>> GetEventsAsync(
        AggregateInfo info, 
        CancellationToken cancellationToken = default)
    {
        return await _store.GetEventsAsync(info, cancellationToken);
    }
    
    // TODO: Перенести в общую сборку.
    private string GetQueueName(Type type)
    {
        return $"{type.Namespace}{type.Name}"
            .Replace('+', '.')
            .ToLowerInvariant();
    }
}