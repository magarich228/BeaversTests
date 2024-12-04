using BeaversTests.S3.MinioProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.Common.S3;

public static class DependencyInjection
{
    private const string S3ProviderConfigurationKey = "S3ProviderType";
    
    public static IServiceCollection AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        var s3ProviderType = configuration.GetRequiredSection(S3ProviderConfigurationKey).Value;
        
        switch (s3ProviderType)
        {
            case MinioProviderConstants.MinioProviderType:
                services.AddS3MinioProvider(configuration);
                break;
            
            default: 
                throw new ArgumentOutOfRangeException($"Unknown S3 provider type: {s3ProviderType}. " +
                                                      $"Check configuration with key: {S3ProviderConfigurationKey}.");
        }

        return services;
    }
}