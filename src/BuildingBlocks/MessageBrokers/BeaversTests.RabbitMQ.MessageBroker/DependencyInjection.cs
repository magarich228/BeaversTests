using BeaversTests.Common.CQRS.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace BeaversTests.RabbitMQ.MessageBroker;

public static class DependencyInjection
{
    private const string RabbitMqConfigurationSectionKey = "RabbitMQ";
    
    public static IServiceCollection AddRabbitMqMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConfiguration = configuration.GetSection(RabbitMqConfigurationSectionKey);
        
        var brokerConfig = new RabbitMQConfiguration();
        rabbitMqConfiguration.Bind(brokerConfig);
        
        services.AddSingleton(brokerConfig);
        
        services.AddRabbitMqServices(new RabbitMqServiceOptions()
        {
            HostName = brokerConfig.Host,
            UserName = brokerConfig.UserName,
            Password = brokerConfig.Password,
            InitialConnectionRetries = brokerConfig.Retries,
            InitialConnectionRetryTimeoutMilliseconds = brokerConfig.RetryTimeoutMilliseconds
        });
        
        services.AddSingleton<IMessageBroker, RabbitMqService>();
        
        return services;
    }
}