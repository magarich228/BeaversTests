namespace BeaversTests.Common.CQRS.Abstractions;

public interface IEventBus
{
    Task PullAsync(CancellationToken cancellationToken = default, params IEvent[] events);
    Task CommitAsync(CancellationToken cancellationToken = default, params IEvent[] events);
    Task CommitAsync(StreamState stream, CancellationToken cancellationToken = default);
}