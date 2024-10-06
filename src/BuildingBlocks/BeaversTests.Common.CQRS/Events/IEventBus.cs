namespace BeaversTests.Common.CQRS.Events;

public interface IEventBus
{
    Task Pull(params IEvent[] events);
    Task Commit(params IEvent[] events);
    Task Commit(StreamState stream);
}