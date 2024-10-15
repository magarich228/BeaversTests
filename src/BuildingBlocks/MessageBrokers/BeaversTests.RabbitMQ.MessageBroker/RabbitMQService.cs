using System.Text;
using System.Text.Json;
using BeaversTests.Common.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BeaversTests.RabbitMQ.MessageBroker;

public class RabbitMqService : IMessageBroker
{
    private readonly IConnectionFactory _factory;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly string _mainExchangeName;
    
    public RabbitMqService(RabbitMQConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        var factory = new ConnectionFactory
        {
            Endpoint = new AmqpTcpEndpoint(configuration.Host, configuration.Port),
            UserName = configuration.UserName,
            Password = configuration.Password,
        };

        _factory = factory;

        _mainExchangeName = configuration.Exchange.Name;
    }
    
    public Task SubscribeAsync(Type type, CancellationToken cancellationToken = default)
    {
        // TODO: type validation

        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            _mainExchangeName, 
            ExchangeType.Fanout);

        var queueName = GetQueueName(type);
        channel.QueueDeclare(
            queueName);

        channel.QueueBind(
            queueName,
            _mainExchangeName,
            string.Empty);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        // TODO: move to method
        consumer.Received += async (sender, @event) =>
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
            
            var body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // TODO: add custom exception, logging
            var deserializedEvent = JsonSerializer.Deserialize(message, type) as IEvent 
                                    ?? throw new ApplicationException($"Can't deserialize event: {message}");
            
            await eventBus.PullAsync(cancellationToken, deserializedEvent);
        };

        channel.BasicConsume(
            queueName,
            true,
            consumer);

        return Task.CompletedTask;
    }

    public async Task SubscribeAsync<TEvent>(CancellationToken cancellationToken = default) 
        where TEvent : IEvent
    {
        await SubscribeAsync(typeof(TEvent), cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent
    {
        var message = JsonSerializer.Serialize(@event);
        var type = GetQueueName(typeof(TEvent));

        await PublishAsync(message, type, cancellationToken);
    }

    public Task PublishAsync(string message, string type, CancellationToken cancellationToken = default)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.ExchangeDeclare(
            _mainExchangeName, 
            ExchangeType.Fanout);
        
        // await channel.QueueDeclareAsync(
        //     type,
        //     cancellationToken: cancellationToken);
        
        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish(
            _mainExchangeName,
            type,
            body: body);

        return Task.CompletedTask;
    }
    
    // TODO: Перенести в общую сборку.
    private string GetQueueName(Type type)
    {
        return $"{type.Namespace}{type.Name}"
            .Replace('+', '.')
            .ToLowerInvariant();
    }
}