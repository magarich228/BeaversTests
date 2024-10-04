using BeaversTests.EventStores;

namespace BeaversTests.Common.CQRS.Events;

public class EventBus : IEventBus
{
    public Task Pull(params IEvent[] events)
    {
        throw new NotImplementedException();
    }

    public Task Commit(params IEvent[] events)
    {
        throw new NotImplementedException();
    }

    public Task Commit(StreamState stream)
    {
        throw new NotImplementedException();
    }
}