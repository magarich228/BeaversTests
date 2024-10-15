using BeaversTests.Common.CQRS;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Postgres.EventStore;
using BeaversTests.RabbitMQ.MessageBroker;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using BeaversTests.TestsManager.Infrastructure.S3Access.Minio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace BeaversTests.TestsManager.Infrastructure;

public static class DependencyInjection
{
    private const string TestsManagerNpgsqlKey = "TestManagerNpgsql";
    private const string MinioS3SectionKey = "S3:Minio";
    
    public static IServiceCollection AddTestsManagerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(TestsManagerNpgsqlKey);
        
        // TODO: перенести
        var brokerConfig = new RabbitMQConfiguration()
        {
            Host = "rabbitmq",
            Port = 5672,
            UserName = "rmuser",
            Password = "rmpassword",
            Exchange = new ExchangeOptions()
            {
                Name = "beaverstests.testsmanager"
            }
        };
        services.AddSingleton(brokerConfig);
        services.AddRabbitMqServices(new RabbitMqServiceOptions()
        {
            HostName = brokerConfig.Host,
            UserName = brokerConfig.UserName,
            Password = brokerConfig.Password
        });
        services.AddSingleton<IMessageBroker, RabbitMqService>();
        
        services.AddDbContext<TestsManagerContext>(options =>
            options.UseNpgsql(connectionString,
                npgOptions => npgOptions.MigrationsAssembly(typeof(TestsManagerContext).Assembly.GetName().Name)));
        
        services.AddScoped<ITestsManagerContext, TestsManagerContext>();

        MinioConfiguration minioConfiguration = new();
        configuration
            .GetSection(MinioS3SectionKey)
            .Bind(minioConfiguration);
        
        services.AddMinio(c => c
            .WithEndpoint(minioConfiguration.Endpoint)
            .WithCredentials(minioConfiguration.AccessKey, minioConfiguration.SecretKey)
            .WithSSL(minioConfiguration.UseSsl));

        services.AddSingleton<ITestsStorageService, TestsStorageService>();
        
        // Перенести
        services.AddPostgresEventStore(configuration);
        services.AddScoped<IEventStore, EventStore>();
        
        return services;
    }
}