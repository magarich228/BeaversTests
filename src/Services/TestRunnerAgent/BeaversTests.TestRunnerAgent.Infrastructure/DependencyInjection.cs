using BeaversTests.TestRunnerAgent.App.Abstractions;
using BeaversTests.TestRunnerAgent.Infrastructure.S3Access.Minio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace BeaversTests.TestRunnerAgent.Infrastructure;

public static class DependencyInjection
{
    private const string MinioS3SectionKey = "S3:Minio";
    
    public static IServiceCollection AddTestRunnerAgentInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        MinioConfiguration minioConfiguration = new();
        configuration
            .GetSection(MinioS3SectionKey)
            .Bind(minioConfiguration);
        
        services.AddMinio(c => c
            .WithEndpoint(minioConfiguration.Endpoint)
            .WithCredentials(minioConfiguration.AccessKey, minioConfiguration.SecretKey)
            .WithSSL(minioConfiguration.UseSsl));

        services.AddSingleton<ITestsStorageReadService, TestsStorageService>();
        
        return services;
    }
}