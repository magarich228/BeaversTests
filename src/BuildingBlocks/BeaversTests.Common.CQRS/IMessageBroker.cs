using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

public interface IMessageBroker
{
    Task SubscribeAsync(Type type, CancellationToken cancellationToken = default);
    Task SubscribeAsync<TEvent>(CancellationToken cancellationToken = default) 
        where TEvent : IEvent;
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent;
    Task PublishAsync(string message, string type, CancellationToken cancellationToken = default);
}