using BeaversTests.Common.S3.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace BeaversTests.S3.MinioProvider;

public static class DependencyInjection
{
    private const string MinioS3SectionKey = "S3:Minio";
    
    public static IServiceCollection AddS3MinioProvider(
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

        services.AddSingleton<IS3Provider, MinioS3Provider>();
        
        return services;
    }
}