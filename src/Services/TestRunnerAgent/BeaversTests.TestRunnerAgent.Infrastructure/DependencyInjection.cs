using BeaversTests.Common.S3;
using BeaversTests.TestRunnerAgent.App.Abstractions;
using BeaversTests.TestRunnerAgent.Infrastructure.S3Access.Minio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestRunnerAgent.Infrastructure;

public static class DependencyInjection
{
    private const string MinioS3SectionKey = "S3:Minio";
    
    public static IServiceCollection AddTestRunnerAgentInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddS3(configuration);

        services.AddSingleton<IDriversStorageReadService, DriversStorageReadService>();
        services.AddSingleton<ITestsStorageReadService, TestsStorageReadService>();
        
        return services;
    }
}