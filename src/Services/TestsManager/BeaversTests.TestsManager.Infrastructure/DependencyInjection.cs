using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using BeaversTests.TestsManager.Infrastructure.S3Access.Minio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace BeaversTests.TestsManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTestsManagerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TestManagerNpgsql");
        
        services.AddDbContext<TestsManagerContext>(options =>
            options.UseNpgsql(connectionString,
                npgOptions => npgOptions.MigrationsAssembly(typeof(TestsManagerContext).Assembly.GetName().Name)));
        
        services.AddScoped<ITestsManagerContext, TestsManagerContext>();

        MinioConfiguration minioConfiguration = new();
        configuration
            .GetSection("S3:Minio")
            .Bind(minioConfiguration);
        
        services.AddMinio(c => c
            .WithEndpoint(minioConfiguration.Endpoint)
            .WithCredentials(minioConfiguration.AccessKey, minioConfiguration.SecretKey)
            .WithSSL(minioConfiguration.UseSsl), 
            ServiceLifetime.Scoped);

        services.AddScoped<ITestsStorageService, TestsStorageService>();
        
        return services;
    }
}