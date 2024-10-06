using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

public interface IMessageBroker
{
    void Subscribe(Type type);
    void Subscribe<TEvent>() where TEvent : IEvent;
    Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    Task Publish(string message, string type);
}