using MediatR;

namespace BeaversTests.Common.CQRS.Events;

public class EventBus(IMediator mediator, IMessageBroker messageBroker) : IEventBus
{
    public virtual async Task Pull(params IEvent[] events)
    {
        foreach (var @event in events)
        {
            await mediator.Publish(@event);
        }
    }

    public async Task Commit(params IEvent[] events)
    {
        foreach (var @event in events)
        {
            await messageBroker.Publish(@event);
        }
    }

    public async Task Commit(StreamState stream)
    {
        // TODO: Проверить, протестить достаточно ли данных
        await messageBroker.Publish(stream.Data, stream.Type);
    }
}