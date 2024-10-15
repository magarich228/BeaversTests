using System.Text;
using System.Text.Json;
using BeaversTests.Common.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BeaversTests.RabbitMQ.MessageBroker;

public class RabbitMqService : IMessageBroker
{
    private readonly IConnectionFactory _factory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMqService> _logger;

    private readonly string _mainExchangeName;
    
    public RabbitMqService(
        RabbitMQConfiguration configuration, 
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitMqService> logger)
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
        _logger = logger;
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
        var queueDeclareOk = channel.QueueDeclare(
            queueName, 
            autoDelete: false,
            exclusive: false,
            durable: true,
            arguments: null);
        
        _logger.LogInformation($"Queue declared {queueDeclareOk.QueueName}");

        channel.QueueBind(
            queueName,
            _mainExchangeName,
            queueName);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        // TODO: move to method
        consumer.Received += async (sender, @event) =>
        {
            _logger.LogInformation($"Received event: {@event.RoutingKey} {@event.Exchange} {@event.ConsumerTag}");
            
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
        
        // var queueName = channel.QueueDeclare(type).QueueName;
        
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