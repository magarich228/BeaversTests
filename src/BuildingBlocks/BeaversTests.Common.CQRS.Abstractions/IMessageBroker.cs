namespace BeaversTests.Common.CQRS.Abstractions;

public interface IMessageBroker
{
    Task SubscribeAsync(Type type, CancellationToken cancellationToken = default);
    Task SubscribeAsync<TEvent>(CancellationToken cancellationToken = default) 
        where TEvent : IEvent;
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent;
    Task PublishAsync(string message, string type, CancellationToken cancellationToken = default);
}