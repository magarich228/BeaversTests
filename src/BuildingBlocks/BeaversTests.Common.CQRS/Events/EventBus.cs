using BeaversTests.Common.CQRS.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BeaversTests.Common.CQRS.Events;

public class EventBus(
    IMediator mediator, 
    IMessageBroker messageBroker,
    ILogger<EventBus> logger) : IEventBus
{
    public virtual async Task PullAsync(CancellationToken cancellationToken = default, params IEvent[] events)
    {
        foreach (var @event in events)
        {
            await mediator.Publish(@event, cancellationToken);
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default, params IEvent[] events)
    {
        foreach (var @event in events)
        {
            await messageBroker.PublishAsync(@event, cancellationToken);
        }
    }

    public async Task CommitAsync(StreamState stream, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Commit stream {stream.Id} {stream.Type}");
        
        // TODO: Проверить, протестить достаточно ли данных
        await messageBroker.PublishAsync(stream.Data, stream.Type, cancellationToken);
    }
}