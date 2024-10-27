using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestRunnerController.Infrastructure;

public static class DependencyInjection
{
    private const string TestRunnerControllerNpgsqlKey = "TestRunnerControllerNpgsqlRead";
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(TestRunnerControllerNpgsqlKey);
        
        services.AddDbContext<TestRunnerControllerContext>(options =>
            options.UseNpgsql(connectionString,
                npgOptions => npgOptions.MigrationsAssembly(typeof(TestRunnerControllerContext).Assembly.GetName().Name)));
        
        services.AddScoped<ITestRunnerControllerContext, TestRunnerControllerContext>();
        
        return services;
    }
}