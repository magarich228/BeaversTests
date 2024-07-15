using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        
        return services;
    }
}