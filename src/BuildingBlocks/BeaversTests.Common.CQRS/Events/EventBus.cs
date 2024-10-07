using MediatR;

namespace BeaversTests.Common.CQRS.Events;

public class EventBus(IMediator mediator, IMessageBroker messageBroker) : IEventBus
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
        // TODO: Проверить, протестить достаточно ли данных
        await messageBroker.PublishAsync(stream.Data, stream.Type, cancellationToken);
    }
}