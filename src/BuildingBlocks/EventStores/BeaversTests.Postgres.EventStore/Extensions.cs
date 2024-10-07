using BeaversTests.Common.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.Postgres.EventStore;

public static class Extensions
{
    private const string NpgsqlEventStoreKey = "EventStoreNpgsql";
    
    public static IServiceCollection AddPostgresEventStore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(NpgsqlEventStoreKey);
        
        services.AddDbContext<PostgresEventStore>(o => 
            o.UseNpgsql(connectionString));

        services.AddScoped<IStore, PostgresEventStore>();
        
        return services;
    }
}