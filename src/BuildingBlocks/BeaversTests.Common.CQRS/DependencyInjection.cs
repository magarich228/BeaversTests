using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.Postgres.EventStore;
using BeaversTests.RabbitMQ.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace BeaversTests.Common.CQRS;

public static class DependencyInjection
{
    private const string EventStoreTypeConfigurationKey = "EventStoreType";
    private const string MessageBrokerTypeConfigurationKey = "MessageBrokerType";
    
    public static IServiceCollection AddCqrs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCqrsCore(configuration)
            .AddEventStore(configuration);

        return services;
    }

    public static IServiceCollection AddCqrsCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBroker(configuration)
            .AddCqrsBusses();

        return services;
    }
    
    private static IServiceCollection AddCqrsBusses(this IServiceCollection services)
    {
        services.AddScoped<ICommandBus, CommandBus>();
        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IEventBus, EventBus>();

        return services;
    }

    private static IServiceCollection AddEventStore(this IServiceCollection services, IConfiguration configuration)
    {
        var eventStoreType = configuration[EventStoreTypeConfigurationKey] ??
                             throw new CqrsInfrastructureException("EventStoreType is not set in configuration.");

        switch (eventStoreType)
        {
            case "Postgres":
                services.AddPostgresEventStore(configuration);
                break;
            
            default: 
                throw new ArgumentOutOfRangeException("Unknown eventStoreTypeValue. value: " + eventStoreType);
        }
        
        services.AddScoped<IEventStore, EventStore>();

        return services;
    }

    private static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var messageBrokerType = configuration[MessageBrokerTypeConfigurationKey] ??
                                throw new CqrsInfrastructureException("MessageBrokerType is not set in configuration.");

        return messageBrokerType switch
        {
            "RabbitMQ" =>
                services.AddRabbitMqMessageBroker(configuration),

            _ => throw new ArgumentOutOfRangeException("Unknown messageBrokerTypeValue. value: " + messageBrokerType)
        };
    }
}