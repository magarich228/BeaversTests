using BeaversTests.Common.CQRS;
using BeaversTests.Common.CQRS.Events;
using RabbitMQ.Client;

namespace BeaversTests.RabbitMQ.MessageBroker;

public class RabbitMQService : IMessageBroker
{
    private readonly IConnectionFactory _factory;
    
    public RabbitMQService(RabbitMQConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            Endpoint = new AmqpTcpEndpoint(configuration.Host, configuration.Port),
            UserName = configuration.UserName,
            Password = configuration.Password
        };

        _factory = factory;
    }
    
    public async void Subscribe(Type type)
    {
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        
        throw new NotImplementedException();
    }

    public void Subscribe<TEvent>() where TEvent : IEvent
    {
        throw new NotImplementedException();
    }

    public Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        throw new NotImplementedException();
    }

    public Task Publish(string message, string type)
    {
        throw new NotImplementedException();
    }
}