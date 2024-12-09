using BeaversTests.Common.S3;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using BeaversTests.TestsManager.Infrastructure.S3Access.Minio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestsManager.Infrastructure;

public static class DependencyInjection
{
    private const string TestsManagerNpgsqlKey = "TestManagerNpgsql";
    
    public static IServiceCollection AddTestsManagerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(TestsManagerNpgsqlKey);
        
        services.AddDbContext<TestsManagerContext>(options =>
            options.UseNpgsql(connectionString,
                npgOptions => npgOptions.MigrationsAssembly(typeof(TestsManagerContext).Assembly.GetName().Name)));
        
        services.AddScoped<ITestsManagerContext, TestsManagerContext>();

        services.AddS3(configuration);

        services.AddSingleton<IDriversStorageWriteService, DriversStorageWriteService>();
        services.AddSingleton<ITestsStorageWriteService, TestsStorageWriteService>();
        
        return services;
    }
}