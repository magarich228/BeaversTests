namespace BeaversTests.Common.CQRS.Events;

public interface IEventBus
{
    Task PullAsync(CancellationToken cancellationToken = default, params IEvent[] events);
    Task CommitAsync(CancellationToken cancellationToken = default, params IEvent[] events);
    Task CommitAsync(StreamState stream, CancellationToken cancellationToken = default);
}