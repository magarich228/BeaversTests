using BeaversTests.Auth.Persistence;
using BeaversTests.Manager.Persistence.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.Manager.Persistence;

public static class DI
{
    private const string TestsManagerNpgsqlKey = "TestsManagerNpgsql";
    
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(TestsManagerNpgsqlKey);
        
        services
            .AddDbContext<TestsManagerDbContext>(options => 
                options.UseNpgsql(connectionString,
                    npgOptions => npgOptions.MigrationsAssembly(typeof(TestsManagerDbContext).Assembly.GetName().Name)));
        
        services.AddScoped<IAuthDbContext, TestsManagerDbContext>();
        
        return services;
    }
}