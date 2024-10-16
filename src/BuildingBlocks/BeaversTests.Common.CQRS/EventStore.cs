using System.Runtime.CompilerServices;
using BeaversTests.Common.CQRS.Abstractions;
using Newtonsoft.Json;

namespace BeaversTests.Common.CQRS;

public class EventStore(IStore store, IEventBus eventBus) : IEventStore
{
    public async Task AppendEventAsync<TAggregate>(
        Guid aggregateId, 
        IEvent @event, 
        int? expectedVersion = null,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new()
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
            Type = GetExchangeName(@event.GetType()),
            Data = JsonConvert.SerializeObject(@event)
        };

        await store.AddAsync(stream, cancellationToken);

        await eventBus.CommitAsync(stream, cancellationToken);
    }

    public async Task<TAggregate> AggregateStreamAsync<TAggregate>(
        AggregateInfo info, 
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new()
    {
        var events = await GetEventsAsync(info, cancellationToken);
        var aggregate = new TAggregate().Empty();
        
        foreach (var @event in events)
        {
            // TODO: custom exception
            // TODO: вынести сериализация, десериализация в общее
            var eventData = JsonConvert.DeserializeObject<IEvent>(@event.Data) ??
                            throw new ApplicationException();
            
            aggregate.Apply(eventData);
            aggregate.Version += 1;
            aggregate.CreatedUtc = @event.CreatedUtc;
        }

        return (TAggregate)aggregate;
    }

    public async IAsyncEnumerable<TAggregate> AggregateStreamAsync<TAggregate>(
        ICollection<Guid> ids, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new()
    {
        foreach (var id in ids)
        {
            yield return await AggregateStreamAsync<TAggregate>(new AggregateInfo()
            {
                Id = id
            }, cancellationToken);
        }
    }

    public async Task StoreAsync<TAggregate>(
        TAggregate aggregate,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new()
    {
        var events = aggregate.DequeueUncommittedEvents();

        var enumerable = events as IEvent[] ?? events.ToArray();
        var initialVersion = aggregate.Version - enumerable.Count();

        foreach (var @event in enumerable)
        {
            initialVersion++;

            // TODO: add validation
            await AppendEventAsync<TAggregate>(aggregate.Id, @event, initialVersion, cancellationToken);
        }
    }

    public async Task StoreAsync<TAggregate>(
        ICollection<TAggregate> aggregates,
        CancellationToken cancellationToken = default) 
        where TAggregate : Aggregate, new()
    {
        foreach (var aggregate in aggregates)
        {
            await StoreAsync(aggregate, cancellationToken);
        }
    }

    public async Task<IEnumerable<StreamState>> GetEventsAsync(
        AggregateInfo info, 
        CancellationToken cancellationToken = default)
    {
        return await store.GetEventsAsync(info, cancellationToken);
    }
    
    // TODO: Перенести в общую сборку.
    private string GetExchangeName(Type type)
    {
        return $"{type.Name}"
            .Replace('+', '.')
            .ToLowerInvariant();
    }
}